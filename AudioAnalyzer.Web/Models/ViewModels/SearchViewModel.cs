using AudioAnalyzer.Web.Models.AudioResponse;

namespace AudioAnalyzer.Web.Models.ViewModels;

public class SearchViewModel: ViewModelBase
{
    public TranscribedResponseJson AudioTranscribedResponse { get; set; }

    public SearchViewModel(TranscribedResponseJson audioTranscribedResponse):base("Search")
    {
        AudioTranscribedResponse = audioTranscribedResponse;
    }
}

public enum SearchErrorCodes
{
    CouldNotTranscribeFile
}
