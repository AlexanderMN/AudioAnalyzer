using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioResponses.TranscribeResponse;

[Serializable]
public class TranscribeResponse
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }
    
    [JsonPropertyName("fileIds")]
    public List<int> FileIds { get; set; }
    
    [JsonPropertyName("fileNames")]
    public List<string> FileNames { get; set; }
    
    [JsonPropertyName("response_code")]
    public int ResponseCode { get; set; }
    
    [JsonPropertyName("response")]
    public object Response { get; set; }
}