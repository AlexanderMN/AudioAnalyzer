using System.ComponentModel.DataAnnotations;

namespace AudioAnalyzer.Web.Models.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Не указано имя пользователя")]
    public string Username { get; set; }
    
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Не указан пароль")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "Не указан Email")]
    public string Email { get; set; }
}
