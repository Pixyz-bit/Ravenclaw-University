using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using _241611JalopFinalQuiz2.Models;

namespace _241611JalopFinalQuiz2.ViewModels;

public class AcademicProgramsViewModel : IAdminSectionViewModel
{
    public ContentSection Section => ContentSection.Programs;
    public int Id { get; set; }

    [Display(Name = "Subject name"), Required, StringLength(150)]
    public string? SubjectName { get; set; }

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Image path or HTTPS URL"), StringLength(500), SafeImage]
    public string? ImagePath { get; set; }

    [BindNever, ValidateNever]
    public List<ProgramsTable> Items { get; set; } = [];

    [BindNever]
    public bool ConfirmDelete { get; set; }

}
