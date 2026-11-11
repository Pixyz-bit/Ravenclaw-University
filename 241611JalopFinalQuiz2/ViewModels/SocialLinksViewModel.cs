using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using _241611JalopFinalQuiz2.Models;

namespace _241611JalopFinalQuiz2.ViewModels;

public class SocialLinksViewModel : IAdminSectionViewModel
{
    public ContentSection Section => ContentSection.SocialLinks;
    public int Id { get; set; }

    [Display(Name = "Platform name"), Required, StringLength(50)]
    public string? PlatformName { get; set; }

    [Display(Name = "HTTPS address"), Required, StringLength(500), HttpsAddress]
    public string? Url { get; set; }

    [Display(Name = "Platform icon"), StringLength(100), SocialIcon]
    public string? IconClass { get; set; }

    [BindNever, ValidateNever]
    public List<SocialLinksTable> Items { get; set; } = [];

    [BindNever]
    public bool ConfirmDelete { get; set; }

}
