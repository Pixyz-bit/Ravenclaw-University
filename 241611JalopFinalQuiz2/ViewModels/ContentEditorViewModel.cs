using System.ComponentModel.DataAnnotations;

namespace _241611JalopFinalQuiz2.ViewModels;

public enum ContentSection { University, Contacts, Values, Photos, Programs, Founders, SocialLinks }

public static class ContentSections
{
    public static string Label(ContentSection section) => section switch
    {
        ContentSection.University => "University information",
        ContentSection.Contacts => "Contact information",
        ContentSection.Values => "Core values",
        ContentSection.Photos => "About gallery",
        ContentSection.Programs => "Academic programs",
        ContentSection.Founders => "Founder information",
        ContentSection.SocialLinks => "Social links",
        _ => "Content"
    };
    public static bool IsSingleton(ContentSection section) => section is ContentSection.University or ContentSection.Contacts;
    public static readonly IReadOnlyDictionary<string, string> Icons = new Dictionary<string, string>
    {
        [""] = "No icon",
        ["fa-brands fa-facebook-f"] = "Facebook",
        ["fa-brands fa-instagram"] = "Instagram",
        ["fa-brands fa-youtube"] = "YouTube",
        ["fa-brands fa-linkedin-in"] = "LinkedIn",
        ["fa-brands fa-x-twitter"] = "X",
        ["fa-solid fa-globe"] = "Website"
    };
}

public class ContentEditorViewModel : IValidatableObject
{
    public int Id { get; set; }
    public ContentSection Section { get; set; }
    [StringLength(150)] public string? Title { get; set; }
    public string? Description { get; set; }
    [StringLength(500), Display(Name = "Image path or HTTPS URL")] public string? ImagePath { get; set; }
    [StringLength(250), Display(Name = "Image description")] public string? AltText { get; set; }
    public string? Mission { get; set; }
    public string? Vision { get; set; }
    [StringLength(500)] public string? Address { get; set; }
    [StringLength(254), EmailAddress] public string? Email { get; set; }
    [StringLength(50), Phone] public string? Telephone { get; set; }
    [StringLength(300), Display(Name = "Office hours")] public string? OfficeHours { get; set; }
    [StringLength(500), Display(Name = "HTTPS address")] public string? Url { get; set; }
    [StringLength(100), Display(Name = "Platform icon")] public string? IconClass { get; set; }
    [DataType(DataType.Date)] public DateOnly? Birthday { get; set; }
    [Display(Name = "Message from the founder")] public string? Message { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Enum.IsDefined(Section) || Id < 0)
            yield return new ValidationResult("Invalid content request.");
        if (Section is ContentSection.Values or ContentSection.Programs or ContentSection.Founders or ContentSection.SocialLinks)
        {
            if (string.IsNullOrWhiteSpace(Title))
                yield return new ValidationResult("A name or title is required.", [nameof(Title)]);
            var max = Section == ContentSection.Values ? 100 : Section == ContentSection.SocialLinks ? 50 : 150;
            if (Title?.Trim().Length > max)
                yield return new ValidationResult($"Use at most {max} characters.", [nameof(Title)]);
        }
        if (Section == ContentSection.Photos && string.IsNullOrWhiteSpace(ImagePath))
            yield return new ValidationResult("Choose an image path or HTTPS URL.", [nameof(ImagePath)]);
        if (!string.IsNullOrWhiteSpace(ImagePath) && !IsSafeImage(ImagePath))
            yield return new ValidationResult("Use a path beginning /images/ without traversal, or an HTTPS image URL.", [nameof(ImagePath)]);
        if (Section == ContentSection.SocialLinks)
        {
            if (!Uri.TryCreate(Url, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
                yield return new ValidationResult("Enter a complete HTTPS URL.", [nameof(Url)]);
            if (!ContentSections.Icons.ContainsKey(IconClass ?? ""))
                yield return new ValidationResult("Choose a supported icon.", [nameof(IconClass)]);
        }
        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila")));
        if (Birthday > today)
            yield return new ValidationResult("Birthday cannot be in the future.", [nameof(Birthday)]);
    }

    public static bool IsSafeImage(string value) =>
        (value.StartsWith("/images/", StringComparison.Ordinal) && !value.Contains("..") && !value.Contains('\\') && !value.Any(char.IsControl))
        || (Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps);
}

public class ContentListViewModel
{
    public ContentSection Section { get; set; }
    public List<ContentEditorViewModel> Items { get; set; } = [];
    public bool Unavailable { get; set; }
    public bool CanAdd => !ContentSections.IsSingleton(Section) || Items.Count == 0;
}
