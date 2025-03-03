using System.ComponentModel.DataAnnotations;

namespace AudioAnalyzer.Web.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Не указано имя пользователя")]
    public string Username { get; set; }
    
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Не указан пароль")]
    public string Password { get; set; }
}
