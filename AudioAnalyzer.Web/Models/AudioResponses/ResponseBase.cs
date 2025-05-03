using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioResponses;

public class ResponseBase
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }
    
    [JsonPropertyName("requestId")]
    public int RequestId { get; set; }
    
    [JsonPropertyName("fileId")]
    public int FileId { get; set; }
    [JsonPropertyName("response_code")]
    public int ResponseCode { get; set; }
}
