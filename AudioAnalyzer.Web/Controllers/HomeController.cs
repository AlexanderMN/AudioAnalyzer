using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AudioAnalyzer.Infrastructure;
using AudioAnalyzer.Infrastructure.Broker;
using AudioAnalyzer.Infrastructure.FileService;
using AudioAnalyzer.Infrastructure.ServiceCommunication;
using AudioAnalyzer.Infrastructure.ServiceCommunication.EndpointService;
using AudioAnalyzer.Web.Models;
using AudioAnalyzer.Web.Models.AudioAnalyzerResponse;
using AudioAnalyzer.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace AudioAnalyzer.Web.Controllers;

[Route("api/[controller]")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IBrokerCommunication _brokerCommunication;
    private readonly IFileServiceCommunication _fileServiceCommunication;
    
    private HomeViewModel _homeViewModel;
    private SearchViewModel _searchViewModel;
    
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
        return View(_homeViewModel);
    }

    [HttpGet]
    [Route("Input")]
    public IActionResult Input()
    {
        return PartialView("Input");
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
        _searchViewModel = new SearchViewModel("");
        
        string? fileName = HttpContext.Session.GetString("FileName");

        if (!string.IsNullOrEmpty(fileName))
        {
            await _brokerCommunication.ExchangeMessagesAsync(topicToSendTo: "Audio-url",
                                                             messageToSend: fileName,
                                                             topicToAwaitFrom: "Transcribe",
                                                             onReceive: OnTranscribe);
        }

        return PartialView("Search", _searchViewModel);
    }
    
    private void OnTranscribe(object state, BrokerEventArgs args)
    {
        int charsWritten = 0;
        Span<char> chars = stackalloc char[4096];
        if (Encoding.UTF8.TryGetChars(args.Message, chars, out charsWritten))
        {
            _searchViewModel.TranscribedText = chars.ToString();
        }
    }
    
    
    //TODO additionally check file extension and permissions to perform action
    [HttpPost]
    [Route("Audio")]
    public async Task<ActionResult> Audio(IFormFile inputFile)
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
            return View(_homeViewModel);
        }
        
        return View(new HomeViewModel(HttpStatusCode.BadRequest));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}