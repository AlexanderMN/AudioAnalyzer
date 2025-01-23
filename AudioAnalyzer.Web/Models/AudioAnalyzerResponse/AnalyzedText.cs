using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioAnalyzerResponse;

[Serializable]
public class AnalyzedText
{
    [JsonPropertyName("text")]
    public string Text { get; set; }
    
    [JsonPropertyName("time")]
    public float TimeInSeconds{ get; set; }
    
    [JsonPropertyName("confidence")]
    public float Confidence{ get; set; }
    
    [JsonPropertyName("words")]
    public List<AnalyzedWord>? Words{ get; set; }
}