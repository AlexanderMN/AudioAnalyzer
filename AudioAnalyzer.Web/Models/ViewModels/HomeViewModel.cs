using System.Net;

namespace AudioAnalyzer.Web.Models.ViewModels;

public class HomeViewModel
{
    public HomeViewModel(HttpStatusCode httpStatusCode = HttpStatusCode.NotFound, string errorMessage = null)
    {
        Response = new HttpResponseMessage(httpStatusCode);
        ErrorMessage = errorMessage;
        TranscribedText = "";
    }

    public HttpResponseMessage Response { get; set; }
    public string ErrorMessage { get; set; }

    public string TranscribedText;
}