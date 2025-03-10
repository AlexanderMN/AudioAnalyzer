using System.Net;
using System.Security.Claims;
using System.Text.Json;
using AudioAnalyzer.Core;
using AudioAnalyzer.Data;
using AudioAnalyzer.Data.Persistence.Models;
using AudioAnalyzer.Infrastructure.Broker;
using AudioAnalyzer.Infrastructure.ServiceCommunication;
using AudioAnalyzer.Web.Hubs;
using AudioAnalyzer.Web.Models.AudioRequest;
using AudioAnalyzer.Web.Models.AudioTranscribeResponse;
using AudioAnalyzer.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AudioAnalyzer.Web.Controllers;

[Authorize]
[Route("api/[controller]")]
public class AudioController : Controller
{
    private readonly IRabbitMqPublisher _rabbitMqPublisher;
    private readonly IFileServiceCommunication _fileServiceCommunication;
    private readonly FileUploadHub _fileUploadHub;
    private readonly AudioFileNameHandler _audioFileNameHandler;
    private readonly DataBaseContext _db;
    
    private readonly HomeViewModel _homeViewModel;
    private SearchViewModel _searchViewModel;
    private TranscribeViewModel _transcribeViewModel;
    public AudioController(ILogger<AudioController> logger, 
                          IRabbitMqPublisher rabbitMqPublisher,
                          IFileServiceCommunication fileServiceCommunication,
                          FileUploadHub fileUploadHub,
                          AudioFileNameHandler audioFileNameHandler,
                          DataBaseContext dataBaseContext
                          )
    {
        _audioFileNameHandler = audioFileNameHandler;
        _rabbitMqPublisher = rabbitMqPublisher;
        _fileServiceCommunication = fileServiceCommunication;
        _fileUploadHub = fileUploadHub;
        _db = dataBaseContext;
        _homeViewModel = new HomeViewModel();
        _searchViewModel = new SearchViewModel(null);
        _transcribeViewModel = new TranscribeViewModel(null);
    }
    
    [HttpGet]
    [Route("")]
    public IActionResult Audio()
    {
        //TODO: map file extensions from appsettings 
        
        //List<string> extensions = [".mp3", ".wav", ".aiff"];

        _homeViewModel.Response = new HttpResponseMessage(HttpStatusCode.OK);
        return View(_homeViewModel);
    }

    [HttpGet]
    [Route("BasicInfo")]
    public IActionResult BasicInfo()
    {
        return PartialView("BasicInfo");
    }

    [HttpGet]
    [Route("Search")]
    public async Task<IActionResult> Search(string filename)
    {
        _searchViewModel = new SearchViewModel(new TranscribedResponseJson());

        var (user, fileName) = await GetUserLastFileNameTupleAsync();

        if (string.IsNullOrEmpty(fileName) || user is null) 
            return PartialView("Search", _searchViewModel);

        await SendTranscribeTask(user: user,
                                    fileName: fileName,
                                    finalTask: BrokerQueues.SearchQueue);

        return PartialView("Search", _searchViewModel);
    }


    [HttpGet]
    [Route("Transcribe")]
    public async Task<IActionResult> Transcribe()
    {
        _transcribeViewModel = new TranscribeViewModel("");
        
        var (user, fileName) = await GetUserLastFileNameTupleAsync();
        
        if (string.IsNullOrEmpty(fileName) || user is null) 
            return PartialView("Transcribe", _transcribeViewModel);

        await SendTranscribeTask(user: user, 
                              fileName: fileName, 
                              finalTask: BrokerQueues.TranscribeQueue);
        
        return PartialView("Transcribe", _transcribeViewModel);
    }

    [HttpGet]
    [Route("Input")]
    public IActionResult Input()
    {
        return PartialView("Input");
    }
    //TODO additionally check file extension and permissions to perform action
    [HttpPost]
    [Route("Input")]
    public async Task<ActionResult> Input(IFormFile inputFile)
    {
        //TODO fix this
        var requestAudioFile = Request.Form.Files[0];
        // if (!knownExtensions.Contains(audioFile.ContentType))
        // {
        //     _homeViewModel.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        //     return View(_homeViewModel);
        // }
        
        
        var audioFile = _audioFileNameHandler.GenerateFileName(requestAudioFile.FileName);
        
        //TODO add file support

        var fileName = $"{audioFile.Name}{audioFile.Extension}";
        
        var ftpResponse = await _fileServiceCommunication.SendDataToFileServiceAsync(
            fileName, 
            requestAudioFile.OpenReadStream());

        if (ftpResponse is null ||
            ftpResponse.StatusCode != FtpStatusCode.ClosingData) 
            return BadRequest();
        
        if (!await TrySaveUserFileAsync(audioFile))
            return BadRequest();
        
        _homeViewModel.Response = new HttpResponseMessage(HttpStatusCode.Accepted);
        return Ok();

    }

    private async Task<User?> GetUserAsync()
    {
        //TODO: add timeout
        var userNameId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!Int32.TryParse(userNameId, out int userId))
            return null;
        
        var cancellationToken = CancellationToken.None;
        
        var user = await _db.Users
                            .Include(u => u.UploadedFiles)
                            .FirstOrDefaultAsync(cancellationToken: cancellationToken, 
                                                 predicate: u => u.Id == userId);
        return user;
    }
    
    private async Task<bool> TrySaveUserFileAsync(AudioFile audioFile)
    {
        var user = await GetUserAsync();
        
        if (user == null)
            return false;
        
        await _db.UploadedFiles.AddAsync(new UploadedFile
        {
            UploadedFileName = audioFile.Name,
            UploadedFileType = audioFile.Extension,
            UserId = user.Id,
            IsProcessed = false
        });
        
        await _db.SaveChangesAsync();
        
        return true;
    }

    private async Task<(User?, string?)> GetUserLastFileNameTupleAsync()
    {
        var user = await GetUserAsync();
        
        var file = user?.UploadedFiles.LastOrDefault();
        if (file == null || file.IsProcessed)
            return (user, string.Empty);
        
        return (user, $"{file.UploadedFileName}{file.UploadedFileType}");
    }

    private async Task SendTranscribeTask(User user, string fileName, string finalTask)
    {
        var transcribeRequest = new AudioTranscribeRequest
        {
            UserId = user.Id,
            AudioFileName = fileName,
            Task = finalTask
        };
            
        var message = JsonSerializer.Serialize(transcribeRequest);
            
        await _rabbitMqPublisher.PublishMessageAsync(message: message, 
                                                     topic: BrokerQueues.AudioFileQueue);
    }
}