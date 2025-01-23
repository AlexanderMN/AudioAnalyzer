using System.Net;

namespace AudioAnalyzer.Web.Models.ViewModels;

public class HomeViewModel
{
    public HomeViewModel(HttpStatusCode httpStatusCode)
    {
        
        Response = new HttpResponseMessage(httpStatusCode);
    }

    public HttpResponseMessage Response { get; set; }
}