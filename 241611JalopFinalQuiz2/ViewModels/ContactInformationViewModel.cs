using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using _241611JalopFinalQuiz2.Models;

namespace _241611JalopFinalQuiz2.ViewModels;

public class ContactInformationViewModel : IAdminSectionViewModel
{
    public ContentSection Section => ContentSection.Contacts;
    public int Id { get; set; }

    [Display(Name = "Address"), StringLength(500)]
    public string? AcademyAddress { get; set; }

    [Display(Name = "Email"), StringLength(254), EmailAddress]
    public string? Email { get; set; }

    [Display(Name = "Telephone"), StringLength(50), Phone]
    public string? Telephone { get; set; }

    [Display(Name = "Office hours"), StringLength(300)]
    public string? OfficeHours { get; set; }

}
