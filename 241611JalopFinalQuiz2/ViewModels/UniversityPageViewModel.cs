using _241611JalopFinalQuiz2.Models;

namespace _241611JalopFinalQuiz2.ViewModels;

public class UniversityPageViewModel
{
    public AcademyInfoTable? University { get; set; }
    public ContactsTable? Contact { get; set; }
    public List<CoreValuesTable> Values { get; set; } = [];
    public List<AboutImagesTable> Photos { get; set; } = [];
    public List<ProgramsTable> Programs { get; set; } = [];
    public List<FounderTable> Founders { get; set; } = [];
    public List<SocialLinksTable> SocialLinks { get; set; } = [];
    public bool Unavailable { get; set; }
}
