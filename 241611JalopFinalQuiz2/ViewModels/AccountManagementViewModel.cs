using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace _241611JalopFinalQuiz2.ViewModels;

public class AccountManagementViewModel
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string? Username { get; set; }
    [Required, RegularExpression("^(Admin|Student)$")] public string Role { get; set; } = "Student";
    [Display(Name = "Active account")] public bool IsActive { get; set; }
    [StringLength(100), Display(Name = "First name")] public string? FirstName { get; set; }
    [StringLength(100), Display(Name = "Middle name")] public string? MiddleName { get; set; }
    [StringLength(100), Display(Name = "Last name")] public string? LastName { get; set; }
    [PastBirthday, DataType(DataType.Date)] public DateOnly? Birthday { get; set; }
    [Display(Name = "Profile image")] public IFormFile? ImageUpload { get; set; }
    [BindNever] public string? ProfileImg { get; set; }
    [StringLength(100)] public string? Search { get; set; }
    [BindNever, ValidateNever] public List<AccountManagementViewModel> Items { get; set; } = [];
}
