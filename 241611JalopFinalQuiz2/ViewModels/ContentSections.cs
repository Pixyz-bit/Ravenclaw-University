namespace _241611JalopFinalQuiz2.ViewModels;

public enum ContentSection { University, Contacts, Programs, Founders }

public static class ContentSections
{
    public static string ControllerName(ContentSection section) => section switch
    {
        ContentSection.University => "UniversityInformation",
        ContentSection.Contacts => "ContactInformation",
        ContentSection.Programs => "AcademicPrograms",
        ContentSection.Founders => "FounderInformation",
        _ => throw new ArgumentOutOfRangeException(nameof(section))
    };
    public static string Label(ContentSection section) => section switch
    {
        ContentSection.University => "Hero and About",
        ContentSection.Contacts => "Contacts",
        ContentSection.Programs => "Academics",
        ContentSection.Founders => "Founder",
        _ => "Content"
    };
}
