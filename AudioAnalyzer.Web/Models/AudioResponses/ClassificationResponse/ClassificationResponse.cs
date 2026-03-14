using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioResponses.ClassificationResponse;

public class ClassificationResponse : ResponseBase
{
    [JsonPropertyName("fileOrderId")]
    public int FileOrderId { get; set; }
    
    [JsonPropertyName("text")]
    public string Text { get; set; }
}
