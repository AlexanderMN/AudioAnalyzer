using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioResponses.SearchResponse;

[Serializable]
public class SearchText
{
    [JsonPropertyName("text")]
    public string Text { get; set; }
    
    [JsonPropertyName("time")]
    public float TimeInSeconds{ get; set; }
    
    [JsonPropertyName("words")]
    public List<SearchWord>? Words{ get; set; }
}