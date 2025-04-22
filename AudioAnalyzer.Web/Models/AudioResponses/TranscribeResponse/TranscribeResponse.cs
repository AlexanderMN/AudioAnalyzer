using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioResponses.TranscribeResponse;

[Serializable]
public class TranscribeResponse
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }
    
    [JsonPropertyName("requestId")]
    public int RequestId { get; set; }
    
    [JsonPropertyName("fileId")]
    public int FileId { get; set; }
    
    [JsonPropertyName("fileOrderId")]
    public int FileOrderId { get; set; }
    
    [JsonPropertyName("response_code")]
    public int ResponseCode { get; set; }
    
    [JsonPropertyName("response")]
    public object Response { get; set; }
}