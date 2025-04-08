using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioResponse;

[Serializable]
public class TranscribedResponseJson
{
    [JsonPropertyName("responses")]
    public List<AudioResponse> AudioResponses { get; set; }
}