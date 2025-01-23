using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioAnalyzerResponse;

[Serializable]
public class AnalyzerResponseJson
{
    [JsonPropertyName("r")]
    public List<AudioResponse> AudioResponses { get; set; }
}