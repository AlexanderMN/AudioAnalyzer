using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioRequests.TranscribeRequest;

[Serializable]
public class TranscribeRequest : RequestBase
{
    [JsonPropertyName("requestId")]
    public int RequestId { get; set; }
}
