using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioResponses.SplitResponse;

public class SplitResponse : ResponseBase
{
    
    [JsonPropertyName("duration")]
    public double Duration { get; set; }
    
    [JsonPropertyName("splitNumber")]
    public int SplitNumber { get; set; }
    
    [JsonPropertyName("response")]
    public object Response { get; set; }
}
