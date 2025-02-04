using AudioAnalyzer.Web.Models.AudioAnalyzerResponse;

namespace AudioAnalyzer.Web.Models.ViewModels;

public class SearchViewModel: ViewModelBase
{
    public AnalyzerResponseJson AudioAnalyzerResponse { get; set; }

    public SearchViewModel(AnalyzerResponseJson audioAnalyzerResponse):base("Search")
    {
        AudioAnalyzerResponse = audioAnalyzerResponse;
    }
}

public enum SearchErrorCodes
{
    CouldNotTranscribeFile
}
