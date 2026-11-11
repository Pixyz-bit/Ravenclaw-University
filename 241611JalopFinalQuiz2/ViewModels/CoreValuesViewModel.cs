using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using _241611JalopFinalQuiz2.Models;

namespace _241611JalopFinalQuiz2.ViewModels;

public class CoreValuesViewModel : IAdminSectionViewModel
{
    public ContentSection Section => ContentSection.Values;
    public int Id { get; set; }

    [Display(Name = "Title"), Required, StringLength(100)]
    public string? Title { get; set; }

    [BindNever, ValidateNever]
    public List<CoreValuesTable> Items { get; set; } = [];

    [BindNever]
    public bool ConfirmDelete { get; set; }

}
