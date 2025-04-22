using System.Net;
using System.Security.Claims;
using AudioAnalyzer.Core;
using AudioAnalyzer.Data;
using AudioAnalyzer.Data.Models;
using AudioAnalyzer.Infrastructure.ServiceCommunication;
using AudioAnalyzer.Web.Models.AudioRequests.SearchRequest;
using AudioAnalyzer.Web.Models.AudioRequests.SplitRequest;
using AudioAnalyzer.Web.Models.AudioRequests.TranscribeRequest;
using AudioAnalyzer.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMqInfrastructure.Broker;

namespace AudioAnalyzer.Web.Controllers;

[Authorize]
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
        
        List<AudioRequest> requests = new List<AudioRequest>();
        
        return PartialView("Requests", requests);
    }
    
    [HttpGet]
    [Route("BasicInfo")]
    public IActionResult BasicInfo()
    {
        return PartialView("BasicInfo");
    }

    [HttpGet]
    [Route("Search")]
    public async Task<IActionResult> Search()
    {
        var searchViewModel = new SearchViewModel();
        return PartialView("Search", searchViewModel);
    }
    
    [HttpPost]
    [Route("Search")]
    public async Task<IActionResult> Search(int fileId)
    {
        var searchViewModel = new SearchViewModel();

        var user = await GetUserAsync();
        if (user == null)
            return Unauthorized("Пользователь не авторизован");

        var request = await _databaseDbContextService.SaveUserRequestAsync(user, AudioRequestType.Search, [fileId]);

        if (request.Id == 0)
            return BadRequest("Не удалось создать запрос");

        var success = await _fileServiceCommunication.CreateRequestFolder(request);
        
        if (!success) 
            return BadRequest("Не удалось создать запрос");

        await _postManager.PostUserRequestToService<SearchRequest>(
            audioRequest: request,
            fileId: fileId,
            callbackQueueName: BrokerQueues.SearchQueue);

        return Ok();
    }


    [HttpGet]
    [Route("Transcribe")]
    public async Task<IActionResult> Transcribe()
    {
        string transcribedText = "";
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
        
        var homeViewModel = new HomeViewModel();
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
            var endpoint = endpoints.First();
            
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
                queueName: BrokerQueues.SplitQueue,
                callbackQueueName: BrokerQueues.SplitResultQueue);
        
        }
        
        homeViewModel.Response = new HttpResponseMessage(HttpStatusCode.Accepted);   
        return Ok();
    }

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