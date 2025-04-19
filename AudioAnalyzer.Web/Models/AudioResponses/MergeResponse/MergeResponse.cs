using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioResponses.MergeResponse;

public class MergeResponse
{
    [JsonPropertyName("requestId")]
    public int RequestId { get; set; }
    
    [JsonPropertyName("fileIds")]
    public List<string> FileNames { get; set; }
    
    [JsonPropertyName("response")]
    public object Response { get; set; }
}
