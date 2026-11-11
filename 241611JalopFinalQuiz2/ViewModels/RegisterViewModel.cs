using System.ComponentModel.DataAnnotations;

namespace _241611JalopFinalQuiz2.ViewModels;

public class RegisterViewModel : IValidatableObject
{
    [Required, StringLength(100), Display(Name = "First name")]
    public string FirstName { get; set; } = "";

    [Required, StringLength(100), Display(Name = "Last name")]
    public string LastName { get; set; } = "";

    [StringLength(100), Display(Name = "Middle name")]
    public string? MiddleName { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? Birthday { get; set; }

    [Required, StringLength(100, MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "Use letters, numbers, periods, underscores, or hyphens.")]
    public string Username { get; set; } = "";

    [Required, StringLength(128, MinimumLength = 8), DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required, Compare(nameof(Password)), DataType(DataType.Password), Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = "";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var manila = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, manila));
        if (Birthday > today)
            yield return new ValidationResult("Birthday cannot be in the future.", [nameof(Birthday)]);
    }
}
