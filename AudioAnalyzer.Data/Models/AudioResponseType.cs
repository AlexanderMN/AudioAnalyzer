using System.ComponentModel.DataAnnotations;

namespace AudioAnalyzer.Data.Models;

public enum AudioResponseType
{
    [Display(Name = "Выполнен")]
    Success = 0,
    [Display(Name = "Выпоняется")]
    Processing = 1,
    [Display(Name = "Ошибка")]
    Error = 2,
}
