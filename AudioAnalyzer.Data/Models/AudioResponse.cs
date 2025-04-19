namespace AudioAnalyzer.Data.Persistence.Models;

public class AudioResponse
{
    public int Id { get; set; }
    
    public bool Success { get; set; }
    
    public string ResponseText { get; set; }
    
    public int FileRequestedEventId { get; set; }
    public FileRequestedEvent FileRequestedEvent { get; set; }
}
