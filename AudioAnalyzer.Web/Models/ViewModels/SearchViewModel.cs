using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
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
