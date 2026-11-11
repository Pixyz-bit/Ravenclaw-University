using System.ComponentModel.DataAnnotations;

namespace _241611JalopFinalQuiz2.ViewModels;

public enum ContentSection { University, Contacts, Values, Photos, Programs, Founders, SocialLinks }

public static class ContentSections
{
    public static string ControllerName(ContentSection section) => section switch
    {
        ContentSection.University => "UniversityInformation",
        ContentSection.Contacts => "ContactInformation",
        ContentSection.Values => "CoreValues",
        ContentSection.Photos => "AboutGallery",
        ContentSection.Programs => "AcademicPrograms",
        ContentSection.Founders => "FounderInformation",
        ContentSection.SocialLinks => "SocialLinks",
        _ => throw new ArgumentOutOfRangeException(nameof(section))
    };
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
