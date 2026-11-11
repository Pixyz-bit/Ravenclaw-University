using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using _241611JalopFinalQuiz2.Models;

namespace _241611JalopFinalQuiz2.ViewModels;

public class AboutGalleryViewModel : IAdminSectionViewModel
{
    public ContentSection Section => ContentSection.Photos;
    public int Id { get; set; }

    [Display(Name = "Image path or HTTPS URL"), Required, StringLength(500), SafeImage]
    public string? ImagePath { get; set; }

    [Display(Name = "Image description"), StringLength(250)]
    public string? AltText { get; set; }

    [BindNever, ValidateNever]
    public List<AboutImagesTable> Items { get; set; } = [];

    [BindNever]
    public bool ConfirmDelete { get; set; }

}
