using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace AudioAnalyzer.Data.Persistence.Models;


public class UploadedFile
{
    public int Id { get; set; }
    public string UploadedFileName { get; set; }
    public string UploadedFileType { get; set; }
    public bool IsProcessed { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }
    public int EndpointId { get; set; }
    public Endpoint Endpoint { get; set; }
    
    public ICollection<FileRequestedEvent> FileRequestedEvents { get; set; }
}
