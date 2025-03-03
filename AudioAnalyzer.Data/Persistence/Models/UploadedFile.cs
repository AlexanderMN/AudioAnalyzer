using System.ComponentModel.DataAnnotations;

namespace AudioAnalyzer.Data.Persistence.Models;

public class UploadedFile
{
    [Key]
    public int UploadedFileId { get; set; }
    public string UploadedFileName { get; set; }
    public string UploadedFileType { get; set; }
    public bool IsProcessed { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }
}
