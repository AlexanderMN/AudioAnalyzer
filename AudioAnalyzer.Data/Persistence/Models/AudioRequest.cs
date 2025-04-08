namespace AudioAnalyzer.Data.Persistence.Models;

public class AudioRequest
{
    public int Id { get; set; }
    public bool IsProcessed { get; set; }
    
    public int AudioRequestTypeId { get; set; }
    public AudioRequestType AudioRequestType { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }
    
    public ICollection<FileRequestedEvent> FileRequestedEvents { get; set; }
}
