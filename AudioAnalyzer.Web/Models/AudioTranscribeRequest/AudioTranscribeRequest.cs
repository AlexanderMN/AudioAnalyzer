using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioRequest;

[Serializable]
public class AudioTranscribeRequest
{
    [JsonPropertyName("filename")]
    public string AudioFileName { get; set; }
    
    [JsonPropertyName("task")]
    public string Task { get; set; }
}
