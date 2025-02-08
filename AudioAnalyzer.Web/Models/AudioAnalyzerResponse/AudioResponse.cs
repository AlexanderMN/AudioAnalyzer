using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioAnalyzerResponse;

[Serializable]
public class AudioResponse
{
    [JsonPropertyName("filename")]
    public string Filename { get; set; }
    
    [JsonPropertyName("response_code")]
    public int ResponseCode { get; set; }
    
    [JsonPropertyName("response")]
    public List<AnalyzedText> AnalyzedTexts { get; set; }
}