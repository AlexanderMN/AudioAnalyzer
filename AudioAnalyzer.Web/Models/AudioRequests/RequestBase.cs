using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioRequests;

public class RequestBase
{
    [JsonPropertyName("userId")]
    public int UserId {get; set;}
    
    [JsonPropertyName("fileId")]
    public int FileId { get; set; }
    
    [JsonPropertyName("task")]
    public string CallbackQueue { get; set; }
    
    [JsonIgnore]
    public string Queue { get; set; }
}
