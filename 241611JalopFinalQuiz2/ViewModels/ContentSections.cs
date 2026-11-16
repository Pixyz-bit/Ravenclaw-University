using System.ComponentModel.DataAnnotations;

namespace _241611JalopFinalQuiz2.ViewModels;

public enum ContentSection { University, Contacts, Values, Photos, Programs, Founders }

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
        _ => throw new ArgumentOutOfRangeException(nameof(section))
    };
    public static string Label(ContentSection section) => section switch
    {
        ContentSection.University => "Hero and About",
        ContentSection.Contacts => "Contacts",
        ContentSection.Values => "Core values",
        ContentSection.Photos => "About gallery",
        ContentSection.Programs => "Academics",
        ContentSection.Founders => "Founder",
        _ => "Content"
    };
    public static bool IsSingleton(ContentSection section) => section is ContentSection.University or ContentSection.Contacts or ContentSection.Founders;

}
