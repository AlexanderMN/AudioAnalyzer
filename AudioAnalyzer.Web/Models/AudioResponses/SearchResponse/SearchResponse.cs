using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioResponses.SearchResponse;

public class SearchResponse : ResponseBase
{
    
    [JsonPropertyName("fileOrderId")]
    public int FileOrderId { get; set; }
    
    [JsonPropertyName("searchText")]
    public SearchText SearchText { get; set; }
}
