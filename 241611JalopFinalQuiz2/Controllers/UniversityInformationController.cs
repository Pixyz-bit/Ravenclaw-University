using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _241611JalopFinalQuiz2.Data;
using _241611JalopFinalQuiz2.Models;
using _241611JalopFinalQuiz2.ViewModels;

namespace _241611JalopFinalQuiz2.Controllers;

[Authorize(Roles = "Admin")]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class UniversityInformationController(UniversityDbContext database, ILogger<UniversityInformationController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var information = await database.AcademyInfoTables.AsNoTracking().OrderBy(x => x.AcademyInfoId).FirstOrDefaultAsync();
            return View(information is null ? new UniversityInformationViewModel() : new UniversityInformationViewModel
            {
                HeroImg = information.HeroImg,
                AcademyDescription = information.AcademyDescription,
                Mission = information.Mission,
                Vision = information.Vision
            });
        }
        catch (Exception exception) { return Unavailable(exception, new UniversityInformationViewModel()); }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(UniversityInformationViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        try
        {
            // A single settings page edits the existing row, or creates the first row.
            await using var transaction = await database.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var information = await database.AcademyInfoTables.OrderBy(x => x.AcademyInfoId).FirstOrDefaultAsync();
            if (information is null)
            {
                information = new AcademyInfoTable();
                database.AcademyInfoTables.Add(information);
            }
            information.HeroImg = Clean(model.HeroImg);
            information.AcademyDescription = Clean(model.AcademyDescription);
            information.Mission = Clean(model.Mission);
            information.Vision = Clean(model.Vision);
            await database.SaveChangesAsync();
            await transaction.CommitAsync();
            TempData["StatusMessage"] = "University information saved. Changes are now available on the university page.";
            return RedirectToAction(nameof(Index), new { id = (int?)null });
        }
        catch (Exception exception) { return Unavailable(exception, model); }
    }

    private IActionResult Unavailable(Exception exception, UniversityInformationViewModel model)
    {
        logger.LogError(exception, "University information could not be loaded or saved.");
        Response.StatusCode = 503;
        ViewData["Unavailable"] = true;
        ModelState.AddModelError("", "University information is temporarily unavailable. Your changes were not confirmed. Please try again.");
        return View("Index", model);
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
