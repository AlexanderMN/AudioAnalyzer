using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioTranscribeResponse;

[Serializable]
public class TranscribedResponseJson
{
    [JsonPropertyName("r")]
    public List<TranscribedResponse> AudioResponses { get; set; }
}