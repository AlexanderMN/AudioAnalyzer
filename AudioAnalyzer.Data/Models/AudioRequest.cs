using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AudioAnalyzer.Data.Models;

public class AudioRequest
{
    public int Id { get; set; }
    [NotMapped]
    public AudioRequestState State { get; set; }
    public AudioRequestType AudioRequestType { get; set; }
    public int UserId { get; set; }
    [JsonIgnore]
    public User User { get; set; }
    
    public int EndpointId { get; set; }
    [JsonIgnore]
    public Endpoint Endpoint { get; set; }
    
    public DateTime CreationDate { get; set; }
    public ICollection<FileRequestedEvent> FileRequestedEvents { get; set; }
}
public enum AudioRequestState
{
    [Display(Name = "Обработка")]
    Processing,
    [Display(Name = "Полностью обработан")]
    Processed,
    [Display(Name = "Обработан с ошибками")]
    ProcessedWithErrors,
    [Display(Name = "Ошибка")]
    Error
}

public enum AudioRequestType
{
    [Display(Name = "Транскрибация")]
    Transcribe,
    [Display(Name = "Поиск")]
    Search,
    [Display(Name = "Суммаризация")]
    Summarize,
    [Display(Name = "Классификация")]
    Classify
}
