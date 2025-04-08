using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioRequest;

[Serializable]
public class UserAudioRequest
{
    [JsonPropertyName("userId")]
    public int UserId {get; set;}
    
    [JsonPropertyName("requestId")]
    public int RequestId { get; set; }
    

    [JsonPropertyName("fileIds")]
    public List<int> FileIds { get; set; }
    
    [JsonPropertyName("folderPath")]
    public string FolderPath { get; set; }
    
    [JsonPropertyName("task")]
    public string CallbackQueue { get; set; }
}
