using AudioAnalyzer.Web.Models.AudioResponses;
using AudioAnalyzer.Web.Models.AudioResponses.TranscribeResponse;

namespace AudioAnalyzer.Web.Models.ViewModels;

public class SearchViewModel: ViewModelBase
{
    public TranscribeResponse AudioTranscribedResponse { get; set; }

    public SearchViewModel(TranscribeResponse transcribedResponse = null):base("Search")
    {
        AudioTranscribedResponse = transcribedResponse;
    }
}

public enum SearchErrorCodes
{
    CouldNotTranscribeFile
}
