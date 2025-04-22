using System.Net;

namespace AudioAnalyzer.Web.Models.ViewModels;

public class HomeViewModel
{
    public HomeViewModel(HttpStatusCode httpStatusCode = HttpStatusCode.NotFound, string errorMessage = null)
    {
        Response = new HttpResponseMessage(httpStatusCode);
        ErrorMessage = errorMessage;
    }

    public string CurrentViewName { get; set; }
    public object ViewData { get; set; }
    public HttpResponseMessage Response { get; set; }
    public string ErrorMessage { get; set; }
    public string FileName { get; set; }
    
    public string UserName { get; set; }
    public byte[] UserImage { get; set; }
    
}