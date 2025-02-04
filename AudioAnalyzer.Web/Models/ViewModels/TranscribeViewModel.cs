namespace AudioAnalyzer.Web.Models.ViewModels;

public class TranscribeViewModel: ViewModelBase
{
    public string TranscribedText;


    public TranscribeViewModel(string transcribedText): base("Transcribe")
    {
        TranscribedText = transcribedText;
    }
}
