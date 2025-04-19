using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioRequests.MergeRequest;

public class MergeRequest
{
    [JsonPropertyName("requestId")]
    public int RequestId { get; set; }
    
    [JsonPropertyName("fileId")]
    public string FileName { get; set; }
    
    [JsonPropertyName("fileIds")]
    public List<string> FileNames { get; set; }
    
    [JsonPropertyName("task")]
    public string CallbackQueue { get; set; }
    
    [JsonPropertyName("response")]
    public object Response { get; set; }
}
