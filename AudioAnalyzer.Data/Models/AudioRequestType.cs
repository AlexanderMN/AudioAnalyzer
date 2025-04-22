using System.ComponentModel.DataAnnotations;

namespace AudioAnalyzer.Data.Models;

public enum AudioRequestType
{
    [Display(Name = "Транскрибация")]
    Transcribe,
    [Display(Name = "Поиск")]
    Search,
    [Display(Name = "Суммаризация")]
    Summarize,
    [Display(Name = "Спектрограмма")]
    Spectrogram
}
