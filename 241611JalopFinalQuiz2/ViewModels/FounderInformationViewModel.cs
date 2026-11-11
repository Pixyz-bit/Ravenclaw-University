using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using _241611JalopFinalQuiz2.Models;

namespace _241611JalopFinalQuiz2.ViewModels;

public class FounderInformationViewModel : IAdminSectionViewModel
{
    public ContentSection Section => ContentSection.Founders;
    public int Id { get; set; }

    [Display(Name = "Founder name"), Required, StringLength(150)]
    public string? FounderName { get; set; }

    [Display(Name = "Birthday"), PastBirthday, DataType(DataType.Date)]
    public DateOnly? Birthday { get; set; }

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Message from the founder")]
    public string? FounderMessage { get; set; }

    [BindNever, ValidateNever]
    public List<FounderTable> Items { get; set; } = [];

    [BindNever]
    public bool ConfirmDelete { get; set; }

}
