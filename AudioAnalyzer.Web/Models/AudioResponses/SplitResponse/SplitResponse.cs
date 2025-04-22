using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioResponses.SplitResponse;

public class SplitResponse
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }
    
    [JsonPropertyName("fileId")]
    public int FileId { get; set; }
    
    [JsonPropertyName("response_code")]
    public int ResponseCode { get; set; }
    
    [JsonPropertyName("duration")]
    public double Duration { get; set; }
    
    [JsonPropertyName("splitNumber")]
    public int SplitNumber { get; set; }
    
    [JsonPropertyName("response")]
    public object Response { get; set; }
}
