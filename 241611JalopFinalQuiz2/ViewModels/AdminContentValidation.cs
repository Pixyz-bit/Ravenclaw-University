using System.ComponentModel.DataAnnotations;

namespace _241611JalopFinalQuiz2.ViewModels;

public interface IAdminSectionViewModel
{
    ContentSection Section { get; }
}

public class SafeImageAttribute : ValidationAttribute
{
    public override bool IsValid(object? value) => value is not string path || string.IsNullOrWhiteSpace(path) || IsSafeImage(path);
    public static bool IsSafeImage(string path) =>
        (path.StartsWith("/images/", StringComparison.Ordinal) && !path.Contains("..") && !path.Contains('\\') && !path.Any(char.IsControl))
        || (Uri.TryCreate(path, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps);
    public SafeImageAttribute() { ErrorMessage = "Use a path beginning /images/ without traversal, or an HTTPS image URL."; }
}

public class HttpsAddressAttribute : ValidationAttribute
{
    public override bool IsValid(object? value) => value is string address && Uri.TryCreate(address, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps;
    public HttpsAddressAttribute() { ErrorMessage = "Enter a complete HTTPS URL."; }
}

public class SocialIconAttribute : ValidationAttribute
{
    public override bool IsValid(object? value) => ContentSections.Icons.ContainsKey(value as string ?? "");
    public SocialIconAttribute() { ErrorMessage = "Choose a supported icon."; }
}

public class PastBirthdayAttribute : ValidationAttribute
{
    public override bool IsValid(object? value) => value is not DateOnly birthday
        || birthday <= DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila")));
    public PastBirthdayAttribute() { ErrorMessage = "Birthday cannot be in the future."; }
}
