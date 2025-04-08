namespace AudioAnalyzer.Data.Persistence.Models;

public class AudioRequestType
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public ICollection<AudioRequest> AudioRequests { get; set; }
}
