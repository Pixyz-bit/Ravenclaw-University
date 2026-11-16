using System.ComponentModel.DataAnnotations;

namespace _241611JalopFinalQuiz2.ViewModels;

public class FounderInformationViewModel : IAdminSectionViewModel
{
    public ContentSection Section => ContentSection.Founders;
    public int Id { get; set; }

    [Display(Name = "Founder name"), Required, StringLength(150)]
    public string? FounderName { get; set; }

    [Display(Name = "Founder Info")]
    public string? Description { get; set; }

    [Display(Name = "Founder Message")]
    public string? FounderMessage { get; set; }

}
