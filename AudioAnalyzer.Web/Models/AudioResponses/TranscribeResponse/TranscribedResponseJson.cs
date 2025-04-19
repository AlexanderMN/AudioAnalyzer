using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioResponses.TranscribeResponse;

[Serializable]
public class TranscribedResponseJson
{
    [JsonPropertyName("responses")]
    public List<TranscribeResponse> AudioResponses { get; set; }
}