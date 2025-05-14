using System.ComponentModel.DataAnnotations;

namespace AudioAnalyzer.Data.Models;


public class UploadedFile
{
    public int Id { get; set; }
    public string UploadedFileName { get; set; }
    public string UploadedFileType { get; set; }
    public FileState FileState { get; set; }
    public long FileSize { get; set; }
    public double Duration { get; set; }
    public int SplitNumber { get; set; }
    public DateTime UploadedDate { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int EndpointId { get; set; }
    public Endpoint Endpoint { get; set; }
    
    public ICollection<FileRequestedEvent> FileRequestedEvents { get; set; }
}

public enum FileState
{
    [Display(Name = "Готов к обработке")]
    Ready = 0,
    [Display(Name = "Предобработка")]
    Prepeprocessing = 1,
    [Display(Name = "Ошибка")]
    Error = 2
    
}
