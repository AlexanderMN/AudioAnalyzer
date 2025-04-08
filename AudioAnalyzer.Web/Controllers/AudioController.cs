using System.Net;
using System.Security.Claims;
using System.Text.Json;
using AudioAnalyzer.Core;
using AudioAnalyzer.Data;
using AudioAnalyzer.Data.Persistence.Models;
using AudioAnalyzer.Data.Persistence.Repositories;
using AudioAnalyzer.Infrastructure.ServiceCommunication;
using AudioAnalyzer.Web.Models.AudioRequest;
using AudioAnalyzer.Web.Models.AudioResponse;
using AudioAnalyzer.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMqInfrastructure.Broker;
using Endpoint = AudioAnalyzer.Data.Persistence.Models.Endpoint;

namespace AudioAnalyzer.Web.Controllers;

[Authorize]
[Route("api/[controller]")]
public class AudioController : Controller
{
    private readonly IRabbitMqPublisher _rabbitMqPublisher;
    private readonly IFileServiceCommunication _fileServiceCommunication;
    private readonly AudioFileNameHandler _audioFileNameHandler;
    private readonly DataBaseContext _db;
    private readonly IRepository<AudioRequest> _audioRequestRepository;

    public AudioController(IRabbitMqPublisher rabbitMqPublisher, 
                           IFileServiceCommunication fileServiceCommunication, 
                           AudioFileNameHandler audioFileNameHandler,
                           DataBaseContext db, 
                           IRepository<AudioRequest> audioRequestRepository)
    {
        _rabbitMqPublisher = rabbitMqPublisher;
        _fileServiceCommunication = fileServiceCommunication;
        _audioFileNameHandler = audioFileNameHandler;
        _db = db;
        _audioRequestRepository = audioRequestRepository;
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
    [Route("BasicInfo")]
    public IActionResult BasicInfo()
    {
        return PartialView("BasicInfo");
    }

    [HttpGet]
    [Route("Search")]
    public async Task<IActionResult> Search(List<int> fileIds)
    {
        var searchViewModel = new SearchViewModel(new TranscribedResponseJson());

        var user = await GetUserAsync();
        if (user == null)
            return Unauthorized();

        await SaveUserRequestAsync(user, 2, fileIds);
        
        await PostTaskToMessageQueueAsync(user: user,
                                    fileIds: fileIds,
                                    queueName: BrokerQueues.AudioFileQueue,
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
        
        await SaveUserRequestAsync(
            user: user,
            requestId: 1,
            fileIds: fileIds);

        await PostTaskToMessageQueueAsync(user: user, 
                              fileIds: fileIds,
                              queueName: BrokerQueues.AudioFileQueue,
                              callbackQueueName: BrokerQueues.TranscribeQueue);
        
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

        await SaveUserRequestAsync(user, 3, fileIds);
        
        await PostTaskToMessageQueueAsync(user: user,
                                     fileIds: fileIds,
                                     queueName: BrokerQueues.AudioFileQueue,
                                     callbackQueueName: BrokerQueues.SpectrogramQueue);
        
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
        //TODO fix this

        var uploadedFiles = new List<UploadedFile>();
        
        foreach (var requestFormFile in Request.Form.Files)
        {
            var audioFile = _audioFileNameHandler.ParseFileName(requestFormFile.FileName);
            if (!SupportedAudioFormats.SetOfSupportedFormats.Contains(audioFile.Extension))
                return BadRequest();

            UploadedFile uploadedFile = new UploadedFile();
            uploadedFile.UploadedFileName = audioFile.Name;
            uploadedFile.UploadedFileType = audioFile.Extension;
            
            var endpoint = await _fileServiceCommunication.SendDataAsFileToFileServerAsync(
                user,
                uploadedFile, 
                requestFormFile.OpenReadStream());

            if (endpoint is null)
            {
                //TODO delete all files from ftp
                return BadRequest();
            }

            uploadedFile.IsProcessed = false;
            uploadedFile.Endpoint = endpoint;
            uploadedFile.EndpointId = endpoint.Id;
            uploadedFile.UserId = user.Id;
            uploadedFiles.Add(uploadedFile);
        }
        
        if (!await TrySaveUserFilesAsync(uploadedFiles))
            return BadRequest();
        
        foreach (var uploadedFile in uploadedFiles)
        {
            await PostTaskToMessageQueueAsync(
                user: user,
                fileIds: [uploadedFile.Id],
                queueName: BrokerQueues.SplitQueue,
                callbackQueueName: BrokerQueues.SplitResultQueue,
                filePath: EndpointService.GetEndpointUri(
                    endpoint: uploadedFile.Endpoint,
                    endpointProtocol: EndpointProtocol.ftp,
                    internalPath: FileServiceCommunication.GetUserInternalFilePath(user, uploadedFile)));
        
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
        
        var cancellationToken = CancellationToken.None;
        
        var user = await _db.Users
                            .Include(u => u.Requests)
                            .FirstOrDefaultAsync(cancellationToken: cancellationToken, 
                                                 predicate: u => u.Id == userId);
        return user;
    }
    
    private async Task<bool> TrySaveUserFilesAsync(List<UploadedFile> uploadedFiles)
    {
        await _db.UploadedFiles.AddRangeAsync(uploadedFiles);
        return await _db.SaveChangesAsync() == uploadedFiles.Count;
    }

    private async Task SaveUserRequestAsync(User user, int requestId, List<int> fileIds)
    {
        AudioRequest audioRequest = new AudioRequest
        {
            AudioRequestTypeId = requestId,
            IsProcessed = false,
            UserId = user.Id,
            FileRequestedEvents = []

        };
        foreach (var fileId in fileIds)
        {
            audioRequest.FileRequestedEvents.Add(new FileRequestedEvent
            {
                AudioRequest = audioRequest,
                UploadedFileId = fileId
            });
        }
        _audioRequestRepository.Create(audioRequest);
        await _audioRequestRepository.SaveAsync();
    }
    
    // private async Task<(User?, string?)> GetUserLastFileNameTupleAsync()
    // {
    //     var user = await GetUserAsync();
    //     
    //     var lastRequest = user?.Requests.LastOrDefault();
    //     
    //     //TODO add multiple file support
    //     var fileRequestedEvents = lastRequest?.FileRequestedEvents;
    //     if (fileRequestedEvents == null)
    //         return (user, string.Empty);
    //
    //     foreach (var fileRequestedEvent in fileRequestedEvents)
    //     {
    //         
    //     }
    //     
    //     return (user, $"{file.UploadedFileName}{file.UploadedFileType}");
    // }

    private async Task PostTaskToMessageQueueAsync(User user,
                                                   List<int> fileIds,
                                                   string queueName, 
                                                   string callbackQueueName,
                                                   string filePath = "")
    {
        var audioRequest = new UserAudioRequest
        {
            UserId = user.Id,
            FileIds = fileIds,
            CallbackQueue = callbackQueueName,
            FilePath = filePath
        };
            
        var message = JsonSerializer.Serialize(audioRequest);
            
        await _rabbitMqPublisher.PublishMessageAsync(message: message, 
                                                     queue: queueName);
    }
}