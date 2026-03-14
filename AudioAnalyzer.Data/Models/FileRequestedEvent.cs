using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AudioAnalyzer.Data.Models;

public class FileRequestedEvent
{
    public int Id { get; set; }
    public int AudioRequestId { get; set; }
    [JsonIgnore]
    public AudioRequest AudioRequest { get; set; }
    public FileRequestedEventState State { get; set; }
    public int UploadedFileId { get; set; }
    [JsonIgnore]
    public UploadedFile UploadedFile { get; set; }
    
    public ICollection<AudioResponse> AudioResponses { get; set; }
}

public enum FileRequestedEventState
{
    [Display(Name = "Выполняется")]
    Processing,
    [Display(Name = "Выполнен")]
    Completed,
    [Display(Name = "Выполнен с ошибками")]
    CompletedWithError,    
    [Display(Name = "Не выполнен")]
    Failed
}
