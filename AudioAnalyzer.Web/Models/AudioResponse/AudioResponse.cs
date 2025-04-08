using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioResponse;

[Serializable]
public class AudioResponse
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }
    
    [JsonPropertyName("filename")]
    public string Filename { get; set; }
    
    [JsonPropertyName("response_code")]
    public int ResponseCode { get; set; }
    
    [JsonPropertyName("response")]
    public object Response { get; set; }
}