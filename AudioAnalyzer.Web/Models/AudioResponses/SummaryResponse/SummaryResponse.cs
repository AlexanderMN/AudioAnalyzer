using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioResponses.SummaryResponse;

public class SummaryResponse : ResponseBase
{
    [JsonPropertyName("fileOrderId")]
    public int FileOrderId { get; set; }
    
    [JsonPropertyName("text")]
    public string Text { get; set; }
}
