using System.ComponentModel.DataAnnotations;

namespace _241611JalopFinalQuiz2.ViewModels;

// Form fields for University information only. The database entity stays database-first.
public class HeroAndAboutViewModel : IAdminSectionViewModel, IValidatableObject
{
    public ContentSection Section => ContentSection.University;
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever]
    public string? HeroImg { get; set; }

    [Display(Name = "Hero image")]
    public IFormFile? ImageUpload { get; set; }

    [Display(Name = "About Heading"), StringLength(150)]
    public string? AboutHeading { get; set; }

    [Display(Name = "About Description")]
    public string? AcademyDescription { get; set; }

    public string? Mission { get; set; }
    public string? Vision { get; set; }

    [Display(Name = "Core Values")]
    public string? CoreValuesText { get; set; }

    public string[] ValueTitles() => (CoreValuesText ?? "").Split('\n')
        .Select(value => value.Trim()).Where(value => value.Length > 0).ToArray();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ValueTitles().Any(value => value.Length > 100))
            yield return new ValidationResult("Each core value must be 100 characters or fewer.", [nameof(CoreValuesText)]);
    }

}
