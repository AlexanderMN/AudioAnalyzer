using System.Net;
using System.Security.Claims;
using AudioAnalyzer.Core;
using AudioAnalyzer.Data;
using AudioAnalyzer.Data.Persistence.Models;
using AudioAnalyzer.Infrastructure.ServiceCommunication;
using AudioAnalyzer.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMqInfrastructure.Broker;

namespace AudioAnalyzer.Web.Controllers;

[Authorize]
[Route("api/[controller]")]
public class AudioController : Controller
{
    private readonly RabbitMqPostManager _postManager;
    private readonly FileServiceCommunication _fileServiceCommunication;
    private readonly AudioFileNameHandler _audioFileNameHandler;
    private readonly DatabaseService _databaseService;
    public AudioController(RabbitMqPostManager postManager, 
                           FileServiceCommunication fileServiceCommunication, 
                           AudioFileNameHandler audioFileNameHandler, 
                           DatabaseService databaseService)
    {
        _postManager = postManager;
        _fileServiceCommunication = fileServiceCommunication;
        _audioFileNameHandler = audioFileNameHandler;
        _databaseService = databaseService;
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
        
        return View(homeViewModel);
    }

    [HttpGet]
    [Route("Files")]
    public async Task<IActionResult> Files()
    {
        var user = await GetUserAsync();
        
        if (user == null)
            return Unauthorized();
        
        List<UploadedFile> audioFiles = _databaseService.UploadedFileRepository
                                                        .GetEntityList(f => f.UserId == user.Id);
        
        return PartialView("Files", audioFiles);
    }
    
    [HttpGet]
    [Route("BasicInfo")]
    public IActionResult BasicInfo()
    {
        return PartialView("BasicInfo");
    }

    [HttpGet]
    [Route("Search")]
    public async Task<IActionResult> Search(int fileId)
    {
        var searchViewModel = new SearchViewModel();

        var user = await GetUserAsync();
        if (user == null)
            return Unauthorized();

        var request = await _databaseService.SaveUserRequestAsync(user, AudioRequestType.Search, [fileId]);

        if (request.Id == 0)
            return BadRequest();

        var success = await _fileServiceCommunication.CreateRequestFolder(request);
        
        if (!success) 
            return BadRequest();

        await _postManager.PostSearchRequest(
            audioRequest: request,
            fileId: fileId,
            callbackQueueName: BrokerQueues.SearchQueue);

        return PartialView("Search", searchViewModel);
    }


    [HttpGet]
    [Route("Transcribe")]
    public async Task<IActionResult> Transcribe(List<int> fileIds)
    {
        var transcribeViewModel = new TranscribeViewModel("");
        
        var user = await GetUserAsync();
        if (user == null)
            return Unauthorized();
        
        var request = await _databaseService.SaveUserRequestAsync(user, AudioRequestType.Transcribe, fileIds);

        if (request.Id == 0)
            return BadRequest();

        var success = await _fileServiceCommunication.CreateRequestFolder(request);
        
        if (!success) 
            return BadRequest();

        foreach (var fileId in fileIds)
        {
            await _postManager.PostTranscribeRequest(
                audioRequest: request,
                fileId: fileId,
                callbackQueueName: BrokerQueues.TranscribeQueue);   
        }
        
        return PartialView("Transcribe", transcribeViewModel);
    }

    [HttpGet]
    [Route("Spectrogram")]
    public async Task<IActionResult> Spectrogram(List<int> fileIds)
    {
        var transcribeViewModel = new TranscribeViewModel("");
        
        var user = await GetUserAsync();
        if (user == null)
            return Unauthorized();

        var request = await _databaseService.SaveUserRequestAsync(user, AudioRequestType.Spectrogram, fileIds);
        if (request.Id == 0)
            return BadRequest();
        
        var success = await _fileServiceCommunication.CreateRequestFolder(request);
        
        if (!success) 
            return BadRequest();
        
        return PartialView("Spectrogram", transcribeViewModel);
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
        //TODO Add file params to db
        foreach (var requestFormFile in Request.Form.Files)
        {
            var audioFile = _audioFileNameHandler.ParseFileName(requestFormFile.FileName);
            if (!SupportedAudioFormats.SetOfSupportedFormats.Contains(audioFile.Extension))
                return BadRequest();
            
            var endpoints = _databaseService.EndpointRepository.GetEntityList(
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
            _databaseService.UploadedFileRepository.Create(uploadedFile);
        }
        await _databaseService.UploadedFileRepository.SaveAsync();

        for (int i = 0; i < uploadedFiles.Count; i++)
        {
            var success = await _fileServiceCommunication.SendDataAsFileToFileServerAsync(
                user,
                uploadedFiles[i],
                Request.Form.Files[i].OpenReadStream());

            if (success)
                continue;

            await _databaseService.RollBackDbFiles(uploadedFiles);
            return BadRequest();
        }

        foreach (var uploadedFile in uploadedFiles)
        {
            await _postManager.PostSplitRequest(
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
        var user = await _databaseService.UserRepository.GetEntity(
            id: userId, 
            includeRelatedEntities: true);
        
        return user;
    }
}