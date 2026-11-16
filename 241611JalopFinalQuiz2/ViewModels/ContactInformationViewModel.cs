using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using _241611JalopFinalQuiz2.Models;

namespace _241611JalopFinalQuiz2.ViewModels;

public class ContactInformationViewModel : IAdminSectionViewModel, IValidatableObject
{
    public ContentSection Section => ContentSection.Contacts;
    public int Id { get; set; }

    [Display(Name = "University Address"), StringLength(500)]
    public string? AcademyAddress { get; set; }

    [Display(Name = "Email"), StringLength(254), EmailAddress]
    public string? Email { get; set; }

    [Display(Name = "Telephone"), StringLength(50), Phone]
    public string? Telephone { get; set; }

    [BindNever, ValidateNever]
    public string? OfficeHours { get; set; }

    [Display(Name = "Time In"), DataType(DataType.Time)]
    public TimeOnly? TimeIn { get; set; }

    [Display(Name = "Time Out"), DataType(DataType.Time)]
    public TimeOnly? TimeOut { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (TimeIn.HasValue != TimeOut.HasValue)
            yield return new ValidationResult("Enter both Time In and Time Out, or leave both empty.", [nameof(TimeIn), nameof(TimeOut)]);
    }

}
