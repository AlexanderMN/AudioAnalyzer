using System.Net;

namespace AudioAnalyzer.Web.Models.ViewModels;

public class HomeViewModel
{
    public HomeViewModel(HttpStatusCode httpStatusCode = HttpStatusCode.NotFound, string errorMessage = null)
    {
        Response = new HttpResponseMessage(httpStatusCode);
        ErrorMessage = errorMessage;
    }

    public HttpResponseMessage Response { get; set; }
    public string ErrorMessage { get; set; }
    public string FileName { get; set; }
    
    public ViewModelBase CurrentViewModel { get; set; }
}