using System.ComponentModel.DataAnnotations;

namespace AudioAnalyzer.Data.Models;

public class AudioResponse
{
    public int Id { get; set; }
    
    public AudioResponseType ResponseType { get; set; }
    
    public string ResponseText { get; set; }
    
    public int OrderId { get; set; }
    
    public int FileRequestedEventId { get; set; }
    public FileRequestedEvent FileRequestedEvent { get; set; }
}

public enum AudioResponseType
{
    [Display(Name = "Выполнен")]
    Success = 0,
    [Display(Name = "Ошибка")]
    Error = 2,
}
