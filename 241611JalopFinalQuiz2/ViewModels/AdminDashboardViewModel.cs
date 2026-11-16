namespace _241611JalopFinalQuiz2.ViewModels;

public class AdminDashboardViewModel
{
    public int Programs { get; set; }
    public int CoreValues { get; set; }
    public int GalleryImages { get; set; }
    public int Accounts { get; set; }
    public bool Unavailable { get; set; }
    public List<AdminContentReminder> Reminders { get; set; } = [];
}

public record AdminContentReminder(string Title, string Detail, string Controller);
