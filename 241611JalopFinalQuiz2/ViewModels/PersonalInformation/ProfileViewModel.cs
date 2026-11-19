using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace _241611JalopFinalQuiz2.ViewModels;

public class ProfileViewModel
{
    [BindNever] public string? Username { get; set; }
    [Required, StringLength(100), Display(Name = "First name")] public string? FirstName { get; set; }
    [StringLength(100), Display(Name = "Middle name")] public string? MiddleName { get; set; }
    [Required, StringLength(100), Display(Name = "Last name")] public string? LastName { get; set; }
    [PastBirthday, DataType(DataType.Date)] public DateOnly? Birthday { get; set; }
    [Display(Name = "Profile image")] public IFormFile? ImageUpload { get; set; }
    [BindNever] public string? ProfileImg { get; set; }
    [BindNever, ValidateNever] public ChangePasswordViewModel Password { get; set; } = new();
}
