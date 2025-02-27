using System.Text.Json.Serialization;

namespace AudioAnalyzer.Web.Models.AudioTranscribeResponse;

[Serializable]
public class TranscribedResponse
{
    [JsonPropertyName("filename")]
    public string Filename { get; set; }
    
    [JsonPropertyName("response_code")]
    public int ResponseCode { get; set; }
    
    [JsonPropertyName("response")]
    public List<TranscribedText> AnalyzedTexts { get; set; }
}