using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioResponses.TranscribeResponse;

[Serializable]
public class TranscribedWord
{
    [JsonPropertyName("word")]
    public string word { get; set; }
    
    [JsonPropertyName("start")]
    public float StartTime { get; set; }
    
    [JsonPropertyName("end")]
    public float EndTime{ get; set; }
    
    [JsonPropertyName("confidence")]
    public float Confidence { get; set; }
}