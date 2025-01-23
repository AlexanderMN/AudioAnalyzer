namespace AudioAnalyzer.Web.Models.ViewModels;

public class HomeViewModel
{
    public HomeViewModel(HttpResponseMessage response)
    {
        Response = response;
    }

    public HttpResponseMessage Response { get; set; }
}