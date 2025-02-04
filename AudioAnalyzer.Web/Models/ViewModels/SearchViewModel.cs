namespace AudioAnalyzer.Web.Models.ViewModels;

public class SearchViewModel
{
    public string TranscribedText;

    public SearchViewModel(string transcribedText)
    {
        TranscribedText = transcribedText;
    }
}
