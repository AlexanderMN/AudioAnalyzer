using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AudioAnalyzer.Infrastructure;
using AudioAnalyzer.Web.Models;
using AudioAnalyzer.Web.Models.AudioAnalyzerResponse;
using AudioAnalyzer.Web.Models.Persistence.Repositories.AudioExtensions;
using AudioAnalyzer.Web.Models.ViewModels;
using AudioAnalyzer.Web.Services.EndpointService;
using Microsoft.AspNetCore.Mvc;
using AudioAnalyzer.Web.Services;
using AudioAnalyzer.Web.Services.FileService;
using Microsoft.Extensions.Primitives;

namespace AudioAnalyzer.Web.Controllers;

[Route("api/[controller]")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IEndpointService<string> _endpointService;
    private readonly IAudioExtensionRepository _audioExtensionRepository;
    private readonly IMessageBroker _messageBroker;
    private readonly IFileService _fileService;
    
    private HttpClient _httpClient;
    
    public HomeController(ILogger<HomeController> logger, 
                          IEndpointService<string> endpointService,
                          IAudioExtensionRepository audioExtensionRepository,
                          IMessageBroker messageBroker,
                          IFileService fileService)
    {
        _logger = logger;
        _endpointService = endpointService;
        _audioExtensionRepository = audioExtensionRepository;
        _messageBroker = messageBroker;
        _fileService = fileService;
    }
    
    [Route("Index")]
    public IActionResult Index()
    {
        return View();
    }
    [Route("Privacy")]
    public IActionResult Privacy()
    {
        return View();
    }

    [Route("")]
    [Route("Audio")]
    [HttpGet]
    public IActionResult Audio()
    {
        //TODO: map file extensions from appsettings 
        
        List<string> extensions = [".mp3", ".wav", ".aiff"];
        
        return View(new HomeViewModel(HttpStatusCode.OK));
    }

    //TODO additionally check file extension and permissions to perform action
    [HttpPost]
    [Route("Audio")]
    public async Task<ActionResult> Audio(IFormFile inputFile)
    {
        var knownExtensions = _audioExtensionRepository.GetAllAudioExtensions();
        var audioExtensionSectionName = _audioExtensionRepository.GetAudioExtensionsSectionName();
        
        //TODO fix this
        var audioFile = Request.Form.Files[0];
        if (!knownExtensions.Contains(audioFile.ContentType))
        {
            return View(new HomeViewModel(HttpStatusCode.BadRequest));
        }
        
        Guid audioId = Guid.NewGuid();
        var fileName = $"{audioId}.wav";
        
        //TODO add file support
        var ftpResponse =  _fileService.UploadFileToFTP(
            uri: _endpointService.GetUriFromEndpointId("FTPServer", EndpointProtocol.ftp,
                $"/audioFiles/{fileName}"),
            stream: audioFile.OpenReadStream());


        if (ftpResponse.StatusCode == FtpStatusCode.ClosingData)
        {
            _messageBroker.Publish("Audio-url", fileName);
            return View(new HomeViewModel(HttpStatusCode.Accepted));
        }
        
        return View(new HomeViewModel(HttpStatusCode.BadRequest));
        
        //TODO: remove magic strings
        
        
        // if (await Task.WhenAny(responseTask, Task.Delay(1000000)) == responseTask)
        // {
        //     var response = await responseTask;
        //     if (response.IsSuccessStatusCode)
        //     {
        //         var responseStream = await response.Content.ReadAsStreamAsync();
        //
        //         byte[] buffer = new byte[responseStream.Length];
        //         int readCount = responseStream.Read(buffer);
        //         
        //         string responseText = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
        //         
        //         //TODO make async
        //         var obj = JsonSerializer.Deserialize<AnalyzerResponseJson>(responseText, JsonSerializerOptions.Default);
        //
        //         if (obj is { } analyzerResponse)
        //         {
        //             
        //         }
        //         
        //         return View();
        //     }
        //     else
        //     {
        //         return View();
        //     }   
        // }
        // else
        // {
        //     return View();
        // }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}