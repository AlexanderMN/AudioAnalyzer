using System.ComponentModel.DataAnnotations;

namespace AudioAnalyzer.Data.Models;

public enum FileState
{
    [Display(Name = "Готов к обработке")]
    Ready = 0,
    [Display(Name = "Предобработка")]
    Prepeprocessing = 1,
    [Display(Name = "Ошибка")]
    Error = 2
    
}
