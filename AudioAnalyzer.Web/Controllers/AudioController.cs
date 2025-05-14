using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using AudioAnalyzer.Core;
using AudioAnalyzer.Data;
using AudioAnalyzer.Data.Models;
using AudioAnalyzer.Infrastructure.ServiceCommunication;
using AudioAnalyzer.Web.Models.AudioRequests.SearchRequest;
using AudioAnalyzer.Web.Models.AudioRequests.TranscribeRequest;
using AudioAnalyzer.Web.Models.AudioResponses.SearchResponse;
using AudioAnalyzer.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using RabbitMqInfrastructure.Broker;

namespace AudioAnalyzer.Web.Controllers;

[Route("[controller]")]
public class AudioController : Controller
{
    private readonly RabbitMqPostManager _postManager;
    private readonly FileServiceCommunication _fileServiceCommunication;
    private readonly AudioFileNameHandler _audioFileNameHandler;
    private readonly DatabaseDbContextService _databaseDbContextService;
    public AudioController(RabbitMqPostManager postManager, 
                           FileServiceCommunication fileServiceCommunication, 
                           AudioFileNameHandler audioFileNameHandler, 
                           DatabaseDbContextService databaseDbContextService)
    {
        _postManager = postManager;
        _fileServiceCommunication = fileServiceCommunication;
        _audioFileNameHandler = audioFileNameHandler;
        _databaseDbContextService = databaseDbContextService;
    }


    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Audio()
    {
        var user = await GetUserAsync();
        if (user == null)
            return Unauthorized();
        
        var homeViewModel = new HomeViewModel();
        
        homeViewModel.Response = new HttpResponseMessage(HttpStatusCode.OK);
        homeViewModel.UserName = user.UserName;
        homeViewModel.CurrentViewName = "Input";
        return View(homeViewModel);
    }

    [HttpGet]
    [Route("Files")]
    public async Task<IActionResult> Files()
    {
        var user = await GetUserAsync();
        
        if (user == null)
            return Unauthorized();
        
        List<UploadedFile> audioFiles = _databaseDbContextService.UploadedFileRepository
                                                        .GetEntityList(f => f.UserId == user.Id);
        
        return PartialView("Files", audioFiles);
    }

    [HttpGet]
    [Route("Requests")]
    public async Task<IActionResult> Requests()
    {
        var user = await GetUserAsync();
        
        if (user == null)
            return Unauthorized();
        
        List<AudioRequest> requests = _databaseDbContextService.AudioRequestRepository
            .GetEntityList(f => f.UserId == user.Id);

        foreach (var request in requests)
        {
            if (request.FileRequestedEvents.All(fre => fre.State == FileRequestedEventState.Completed))
            {
                request.State = AudioRequestState.Processed;
                continue;
            }

            if (request.FileRequestedEvents.All(fre => fre.State == FileRequestedEventState.Failed))
            {
                request.State = AudioRequestState.Processed;
                continue;
            }

            if (request.FileRequestedEvents.Any(fre => fre.State == FileRequestedEventState.Processing))
            {
                request.State = AudioRequestState.Processing;
                continue;
            }
            
            request.State = AudioRequestState.ProcessedWithErrors;
        }
        
        return PartialView("Requests", requests);
    }
    
    [HttpGet]
    [Route("BasicInfo")]
    public IActionResult BasicInfo()
    {
        return PartialView("BasicInfo");
    }

    [HttpGet]
    [Route("Summary")]
    public async Task<IActionResult> Summary()
    {
        var user = await GetUserAsync();
        return PartialView("Summary", user);
    }
    
    [HttpGet]
    [Route("Search")]
    public async Task<IActionResult> Search(int requestId, int fileId)
    {
        var fileRequestedEvent = await _databaseDbContextService
            .GetFileRequestedEventByIndex(requestId, fileId, true);

        var responses = fileRequestedEvent.AudioResponses
                                          .OrderBy(resp => resp.OrderId)
                                          .Select(resp => resp.ResponseText).ToList();

        var fullSearchResponse = new SearchResponse
        {
            SearchText = new SearchText
            {
                
            }
        };

        var finalTextBuilder = new StringBuilder();
        var finalTextTime = 0.0f;
        
        var searchWords = new List<SearchWord>();

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic, UnicodeRanges.Arabic)
        };
        
        foreach (var response in responses)
        {
            var searchText = JsonSerializer.Deserialize<SearchText>(response, options);

            finalTextBuilder.Append(searchText.Text);
            
            searchWords.AddRange(searchText.Words.Select(w => new SearchWord
            {
                word = w.word,
                StartTime = w.StartTime + finalTextTime,
                EndTime = w.EndTime + finalTextTime,
            }));
            
            finalTextTime += searchText.TimeInSeconds;
        }
        
        fullSearchResponse.SearchText.Text = finalTextBuilder.ToString();
        fullSearchResponse.SearchText.TimeInSeconds = finalTextTime;
        fullSearchResponse.SearchText.Words = searchWords;
        
        
        var searchViewModel = new SearchViewModel(fullSearchResponse);
        return PartialView("Search", searchViewModel);
    }
    
    [HttpPost]
    [Route("Search")]
    public async Task<IActionResult> Search(List<int> fileIds)
    {
        var user = await GetUserAsync();
        if (user == null)
            return Unauthorized("Пользователь не авторизован");

        var request = await _databaseDbContextService.SaveUserRequestAsync(user, AudioRequestType.Search, fileIds);

        if (request.Id == 0)
            return BadRequest("Не удалось создать запрос");
        
        var uploadedFiles = _databaseDbContextService.UploadedFileRepository
                                                     .GetEntityList(f => fileIds.Contains(f.Id));
        
        foreach (var file in uploadedFiles)
        {
            for (int i = 0; i < file.SplitNumber; i++)
            {
                await _postManager.PostUserRequestToService<SearchRequest>(
                    audioRequest: request,
                    fileId: file.Id,
                    callbackQueueName: BrokerQueues.SearchQueue);
            }
        }
        

        return Ok();
    }


    [HttpGet]
    [Route("Transcribe")]
    public async Task<IActionResult> Transcribe(int requestId, int fileId)
    {
        var fileRequestedEvent = await _databaseDbContextService.GetFileRequestedEventByIndex(requestId, fileId, true);

        if (fileRequestedEvent == null)
            return BadRequest("Запрос еще не обработан");
        var transcribedText = string.Join("", fileRequestedEvent.AudioResponses
                                                  .OrderBy(resp => resp.OrderId)
                                                  .Select(resp => resp.ResponseText));
        if (fileRequestedEvent == null)
            return BadRequest("Request not found");
        
        var transcribeViewModel = new TranscribeViewModel(transcribedText);
        return PartialView("Transcribe", transcribeViewModel);
    }
    
    [HttpPost]
    [Route("Transcribe")]
    public async Task<IActionResult> Transcribe(List<int> fileIds)
    {
        var user = await GetUserAsync();
        if (user == null || fileIds.Count == 0)
            return Unauthorized("Пользователь не авторизован");
        
        var request = await _databaseDbContextService.SaveUserRequestAsync(user, AudioRequestType.Transcribe, fileIds);
        if (request.Id == 0)
            return BadRequest("Не удалось создать запрос");

        var uploadedFiles = _databaseDbContextService.UploadedFileRepository
                                                     .GetEntityList(f => fileIds.Contains(f.Id));
        
        foreach (var file in uploadedFiles)
        {
            for (int i = 0; i < file.SplitNumber; i++)
            {
                await _postManager.PostUserRequestToService<TranscribeRequest>(
                    audioRequest: request,
                    fileId: file.Id,
                    fileOrderId: i,
                    callbackQueueName: BrokerQueues.TranscribeQueue);
            }
        }

        return Ok();
    }
    
    [HttpGet]
    [Route("Input")]
    public IActionResult Input()
    {
        return PartialView("Input");
    }

    [HttpPost]
    [Route("Input")]
    public async Task<ActionResult> Input(IFormFile inputFile)
    {
        var user = await GetUserAsync();
        if (user == null)
            return Unauthorized();
        var uploadedFiles = new List<UploadedFile>();
        foreach (var requestFormFile in Request.Form.Files)
        {
            var audioFile = _audioFileNameHandler.ParseFileName(requestFormFile.FileName);
            if (!SupportedAudioFormats.SetOfSupportedFormats.Contains(audioFile.Extension))
                return BadRequest();
            
            var endpoints = _databaseDbContextService.EndpointRepository.GetEntityList(
                (e => e.EndPointType == EndPointType.FTPServer)
            );
            if (endpoints.Count == 0)
                return BadRequest();
            var endpoint = endpoints.MaxBy(e => e.SpaceLeft);
            
            UploadedFile uploadedFile = new UploadedFile
            {
                UploadedFileName = audioFile.Name,
                UploadedFileType = audioFile.Extension,
                FileState = FileState.Prepeprocessing,
                UploadedDate = DateTime.UtcNow,
                Endpoint = endpoint,
                EndpointId = endpoint.Id,
                UserId = user.Id
            };
            uploadedFiles.Add(uploadedFile);
            _databaseDbContextService.UploadedFileRepository.Create(uploadedFile);
        }
        await _databaseDbContextService.UploadedFileRepository.SaveAsync();

        for (int i = 0; i < uploadedFiles.Count; i++)
        {
            var success = await _fileServiceCommunication.SendDataAsFileToFileServerAsync(
                user,
                uploadedFiles[i],
                Request.Form.Files[i].OpenReadStream());

            if (success)
                continue;

            await _databaseDbContextService.RollBackDbFiles(uploadedFiles);
            return BadRequest();
        }

        foreach (var uploadedFile in uploadedFiles)
        {
            await _postManager.PostSystemRequestToService(
                user: user,
                fileId: uploadedFile.Id,
                queueName: BrokerQueues.PreprocessQueue,
                callbackQueueName: BrokerQueues.PreprocessResultQueue);
        
        }
        return Ok();
    }

    [HttpGet]
    [Route("AudioFile")]
    public async Task<IActionResult> AudioFile(int fileId)
    {
        var uploadedFile = await _databaseDbContextService.UploadedFileRepository.GetEntity(fileId, true);
        
        if (uploadedFile == null)
            return BadRequest("Файл не найден");
        await using var fileStream = await _fileServiceCommunication.GetDataFromFileServerAsync(uploadedFile);
        byte[] wavArray = fileStream!.ToArray();
        return File(wavArray,  "application/octet-stream");
    }
    [NonAction]
    private async Task<User?> GetUserAsync()
    {
        //TODO: add timeout
        var userNameId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!int.TryParse(userNameId, out int userId))
            return null;
        var user = await _databaseDbContextService.UserRepository.GetEntity(
            id: userId, 
            includeRelatedEntities: true);
        
        return user;
    }
}