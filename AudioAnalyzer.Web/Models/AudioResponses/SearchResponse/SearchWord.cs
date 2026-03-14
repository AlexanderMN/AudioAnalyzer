using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioResponses.SearchResponse;

[Serializable]
public class SearchWord
{
    [JsonPropertyName("word")]
    public string word { get; set; }
    
    [JsonPropertyName("start")]
    public float StartTime { get; set; }
    
    [JsonPropertyName("end")]
    public float EndTime{ get; set; }
}