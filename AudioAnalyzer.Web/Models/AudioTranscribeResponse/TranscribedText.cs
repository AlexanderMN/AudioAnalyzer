using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioTranscribeResponse;

[Serializable]
public class TranscribedText
{
    [JsonPropertyName("text")]
    public string Text { get; set; }
    
    [JsonPropertyName("time")]
    public float TimeInSeconds{ get; set; }
    
    [JsonPropertyName("confidence")]
    public float Confidence{ get; set; }
    
    [JsonPropertyName("words")]
    public List<TranscribedWord>? Words{ get; set; }
}