using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using AudioAnalyzer.Infrastructure.Broker;
using AudioAnalyzer.Infrastructure.FileService;
using AudioAnalyzer.Infrastructure.ServiceCommunication;
using AudioAnalyzer.Web.Hubs;
using AudioAnalyzer.Web.Models;
using AudioAnalyzer.Web.Models.AudioAnalyzerResponse;
using AudioAnalyzer.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AudioAnalyzer.Web.Controllers;

[Route("api/[controller]")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IRabbitMqPublisher _rabbitMqPublisher;
    private readonly IFileServiceCommunication _fileServiceCommunication;
    private readonly FileUploadHub _fileUploadHub;
    
    private readonly HomeViewModel _homeViewModel;
    private SearchViewModel _searchViewModel;
    private TranscribeViewModel _transcribeViewModel;
    public HomeController(ILogger<HomeController> logger, 
                          IRabbitMqPublisher rabbitMqPublisher,
                          IFileServiceCommunication fileServiceCommunication,
                          FileUploadHub fileUploadHub
                          )
    {
        _logger = logger;
        _rabbitMqPublisher = rabbitMqPublisher;
        _fileServiceCommunication = fileServiceCommunication; ;
        _homeViewModel = new HomeViewModel();
        _fileUploadHub = fileUploadHub;
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

        _homeViewModel.Response = new HttpResponseMessage(HttpStatusCode.OK);
        return View(_homeViewModel);
    }

    [HttpGet]
    [Route("Audio/BasicInfo")]
    public IActionResult BasicInfo()
    {
        return PartialView("BasicInfo");
    }

    [HttpGet]
    [Route("Audio/Search")]
    public async Task<IActionResult> Search(string filename)
    {
        _searchViewModel = new SearchViewModel(new AnalyzerResponseJson());
        
        string? fileName = HttpContext.Session.GetString("FileName");

        if (!string.IsNullOrEmpty(fileName))
        {
            await _rabbitMqPublisher.PublishMessageAsync(fileName, "Audio-url");
            
        }

        return PartialView("Search", _searchViewModel);
    }


    [HttpGet]
    [Route("Audio/Transcribe")]
    public async Task<IActionResult> Transcribe()
    {
        _transcribeViewModel = new TranscribeViewModel("");
        
        string? fileName = HttpContext.Session.GetString("FileName");

        if (!string.IsNullOrEmpty(fileName))
        {
            await _rabbitMqPublisher.PublishMessageAsync(fileName, "Audio-url");
        }
        
        return PartialView("Transcribe", _transcribeViewModel);
    }

    [HttpGet]
    [Route("Audio/Input")]
    public IActionResult Input()
    {
        return PartialView("Input");
    }
    //TODO additionally check file extension and permissions to perform action
    [HttpPost]
    [Route("Audio/Input")]
    public async Task<ActionResult> Input(IFormFile inputFile)
    {
        //TODO fix this
        var audioFile = Request.Form.Files[0];
        // if (!knownExtensions.Contains(audioFile.ContentType))
        // {
        //     _homeViewModel.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        //     return View(_homeViewModel);
        // }
        
        Guid audioId = Guid.NewGuid();
        var fileName = $"{audioId}.wav";
        
        HttpContext.Session.SetString("FileName", fileName);
        
        //TODO add file support

        var ftpResponse = await _fileServiceCommunication.SendDataToFileServiceAsync(
            fileName, 
            audioFile.OpenReadStream());

        if (ftpResponse.StatusCode == FtpStatusCode.ClosingData)
        {
            _homeViewModel.Response = new HttpResponseMessage(HttpStatusCode.Accepted);
            return Ok(fileName);
        }
        
        return BadRequest();
    }
}