using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using AudioAnalyzer.Infrastructure.Broker;
using AudioAnalyzer.Infrastructure.ServiceCommunication;
using AudioAnalyzer.Web.Models;
using AudioAnalyzer.Web.Models.AudioAnalyzerResponse;
using AudioAnalyzer.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AudioAnalyzer.Web.Controllers;

[Route("api/[controller]")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IBrokerCommunication _brokerCommunication;
    private readonly IFileServiceCommunication _fileServiceCommunication;
    
    private readonly HomeViewModel _homeViewModel;
    private SearchViewModel _searchViewModel;
    private TranscribeViewModel _transcribeViewModel;
    public HomeController(ILogger<HomeController> logger, 
                          IBrokerCommunication brokerCommunication,
                          IFileServiceCommunication fileServiceCommunication
                          )
    {
        _logger = logger;
        _brokerCommunication = brokerCommunication;
        _fileServiceCommunication = fileServiceCommunication;
        
        _homeViewModel = new HomeViewModel();
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
        _homeViewModel.CurrentViewModel = new InputViewModel();
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
    public async Task<IActionResult> Search()
    {
        _searchViewModel = new SearchViewModel(new AnalyzerResponseJson());
        
        string? fileName = HttpContext.Session.GetString("FileName");

        if (!string.IsNullOrEmpty(fileName))
        {
            await _brokerCommunication.ExchangeMessagesAsync(topicToSendTo: "Audio-url",
                                                             messageToSend: fileName,
                                                             topicToAwaitFrom: "Transcribe",
                                                             onReceive: OnSearch);
        }

        return PartialView("Search", _searchViewModel);
    }

    public void OnSearch(object state, BrokerEventArgs args)
    {
        string text = Encoding.UTF8.GetString(args.Message, 0, args.Message.Length);
        var jsonResponse = JsonSerializer.Deserialize<AnalyzerResponseJson>(text);

        _searchViewModel.AudioAnalyzerResponse = jsonResponse ?? new AnalyzerResponseJson();
    }

    [HttpGet]
    [Route("Audio/Transcribe")]
    public async Task<IActionResult> Transcribe()
    {
        _transcribeViewModel = new TranscribeViewModel("");
        
        string? fileName = HttpContext.Session.GetString("FileName");

        if (!string.IsNullOrEmpty(fileName))
        {
            await _brokerCommunication.ExchangeMessagesAsync(topicToSendTo: "Audio-url",
                                                             messageToSend: fileName,
                                                             topicToAwaitFrom: "Transcribe",
                                                             onReceive: OnTranscribe);
        }
        
        return PartialView("Transcribe", _transcribeViewModel);
    }
    
    private void OnTranscribe(object state, BrokerEventArgs args)
    {
        string text = Encoding.UTF8.GetString(args.Message, 0, args.Message.Length);
        var jsonResponse = JsonSerializer.Deserialize<AnalyzerResponseJson>(text);

        if (jsonResponse != null)
        {
            _transcribeViewModel.TranscribedText = jsonResponse.AudioResponses[0].AnalyzedTexts[0].Text;
        }
        else
        {
            _transcribeViewModel.TranscribedText = text;
        }
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
            return View("Audio", _homeViewModel);
        }
        
        return View("Audio", _homeViewModel);
    }
}