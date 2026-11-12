using System.ComponentModel.DataAnnotations;

namespace _241611JalopFinalQuiz2.ViewModels;

// Form fields for University information only. The database entity stays database-first.
public class UniversityInformationViewModel : IAdminSectionViewModel
{
    public ContentSection Section => ContentSection.University;
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever]
    public string? HeroImg { get; set; }

    [Display(Name = "Hero image")]
    public IFormFile? ImageUpload { get; set; }

    [Display(Name = "University description")]
    public string? AcademyDescription { get; set; }

    public string? Mission { get; set; }
    public string? Vision { get; set; }

}
