using System.ComponentModel.DataAnnotations;

namespace _241611JalopFinalQuiz2.ViewModels;

public class LoginViewModel
{
    [Required, StringLength(100)]
    public string Username { get; set; } = "";

    [Required, DataType(DataType.Password), StringLength(128)]
    public string Password { get; set; } = "";

    public string? ReturnUrl { get; set; }
}
