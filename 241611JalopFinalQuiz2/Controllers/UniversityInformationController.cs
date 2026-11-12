using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _241611JalopFinalQuiz2.Services;
using _241611JalopFinalQuiz2.Data;
using _241611JalopFinalQuiz2.Models;
using _241611JalopFinalQuiz2.ViewModels;

namespace _241611JalopFinalQuiz2.Controllers;

[Authorize(Roles = "Admin")]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class UniversityInformationController(UniversityDbContext database, ImageUploadService images, ILogger<UniversityInformationController> logger) : Controller
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

    [HttpPost, ValidateAntiForgeryToken, RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> Index(UniversityInformationViewModel model)
    {
        string? uploadedPath = null;
        try
        {
            model.HeroImg = await database.AcademyInfoTables.AsNoTracking().OrderBy(x => x.AcademyInfoId).Select(x => x.HeroImg).FirstOrDefaultAsync();
            var imageError = await images.ValidateAsync(model.ImageUpload);
            if (imageError is not null) ModelState.AddModelError(nameof(model.ImageUpload), imageError);
            if (!ModelState.IsValid) return View(model);
            // A single settings page edits the existing row, or creates the first row.
            await using var transaction = await database.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var information = await database.AcademyInfoTables.OrderBy(x => x.AcademyInfoId).FirstOrDefaultAsync();
            if (information is null)
            {
                information = new AcademyInfoTable();
                database.AcademyInfoTables.Add(information);
            }
            if (model.ImageUpload is not null)
            {
                uploadedPath = await images.SaveAsync(model.ImageUpload);
                information.HeroImg = uploadedPath;
            }
            information.AcademyDescription = Clean(model.AcademyDescription);
            information.Mission = Clean(model.Mission);
            information.Vision = Clean(model.Vision);
            await database.SaveChangesAsync();
            await transaction.CommitAsync();
            uploadedPath = null; // The saved row now owns this upload.
            TempData["StatusMessage"] = "University information saved. Changes are now available on the university page.";
            return RedirectToAction(nameof(Index), new { id = (int?)null });
        }
        catch (Exception exception) { images.Discard(uploadedPath); return Unavailable(exception, model); }
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
