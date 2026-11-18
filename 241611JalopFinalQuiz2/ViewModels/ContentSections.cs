namespace _241611JalopFinalQuiz2.ViewModels;

// Preserve legacy numeric links; value 3 belonged to the retired gallery.
public enum ContentSection { University = 0, Contacts = 1, Values = 2, Programs = 4, Founders = 5 }

public static class ContentSections
{
    public static string ControllerName(ContentSection section) => section switch
    {
        ContentSection.University => "UniversityInformation",
        ContentSection.Contacts => "ContactInformation",
        ContentSection.Values => "CoreValues",
        ContentSection.Programs => "AcademicPrograms",
        ContentSection.Founders => "FounderInformation",
        _ => throw new ArgumentOutOfRangeException(nameof(section))
    };
    public static string Label(ContentSection section) => section switch
    {
        ContentSection.University => "Hero and About",
        ContentSection.Contacts => "Contacts",
        ContentSection.Values => "Core values",
        ContentSection.Programs => "Academics",
        ContentSection.Founders => "Founder",
        _ => "Content"
    };
    public static bool IsSingleton(ContentSection section) => section is ContentSection.University or ContentSection.Contacts or ContentSection.Founders;

}
