using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioResponses.PreprocessResponse;

public class PreprocessResponse : ResponseBase
{
    
    [JsonPropertyName("duration")]
    public double Duration { get; set; }
    
    [JsonPropertyName("splitNumber")]
    public int SplitNumber { get; set; }
    
    [JsonPropertyName("response")]
    public object Response { get; set; }
}
