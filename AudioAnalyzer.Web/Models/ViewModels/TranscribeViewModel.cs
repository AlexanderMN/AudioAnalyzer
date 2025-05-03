using AudioAnalyzer.Data.Models;

namespace AudioAnalyzer.Web.Models.ViewModels;

public class TranscribeViewModel
{
    public string TranscribedText { get; set; }

    public TranscribeViewModel(string transcribedText)
    {
        TranscribedText = transcribedText;
    }
}
