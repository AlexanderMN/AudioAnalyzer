using System.Text.Json;
using AudioAnalyzer.Web.Models.AudioResponses;
using AudioAnalyzer.Web.Models.AudioResponses.SearchResponse;
using AudioAnalyzer.Web.Models.AudioResponses.TranscribeResponse;

namespace AudioAnalyzer.Web.Models.ViewModels;

public class SearchViewModel
{
    public string SearchResponse;

    public SearchViewModel(SearchResponse searchResponse)
    {
        SearchResponse = JsonSerializer.Serialize(searchResponse.SearchText);;
    }
}
