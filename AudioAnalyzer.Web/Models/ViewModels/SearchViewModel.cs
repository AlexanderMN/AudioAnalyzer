using AudioAnalyzer.Web.Models.AudioResponses;
using AudioAnalyzer.Web.Models.AudioResponses.TranscribeResponse;

namespace AudioAnalyzer.Web.Models.ViewModels;

public class SearchViewModel: ViewModelBase
{
    public TranscribedResponseJson AudioTranscribedResponse { get; set; }

    public SearchViewModel(TranscribedResponseJson audioTranscribedResponse = null):base("Search")
    {
        AudioTranscribedResponse = audioTranscribedResponse;
    }
}

public enum SearchErrorCodes
{
    CouldNotTranscribeFile
}
