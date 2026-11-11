using System.ComponentModel.DataAnnotations;

namespace _241611JalopFinalQuiz2.ViewModels;

// Form fields for University information only. The database entity stays database-first.
public class UniversityInformationViewModel : IAdminSectionViewModel
{
    public ContentSection Section => ContentSection.University;
    [Display(Name = "Hero image"), StringLength(500), SafeImage]
    public string? HeroImg { get; set; }

    [Display(Name = "University description")]
    public string? AcademyDescription { get; set; }

    public string? Mission { get; set; }
    public string? Vision { get; set; }

}
