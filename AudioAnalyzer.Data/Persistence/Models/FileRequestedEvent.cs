namespace AudioAnalyzer.Data.Persistence.Models;

public class FileRequestedEvent
{
    public int Id { get; set; }
    
    public string UploadedFileName { get; set; }
    
    public int AudioRequestId { get; set; }
    public AudioRequest AudioRequest { get; set; }
    
    public int UploadedFileId { get; set; }
    public UploadedFile UploadedFile { get; set; }
}
