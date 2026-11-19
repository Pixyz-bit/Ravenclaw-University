using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _241611JalopFinalQuiz2.Data;
using _241611JalopFinalQuiz2.ViewModels;

namespace _241611JalopFinalQuiz2.Controllers;

[Authorize(Roles = "Admin")]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class FounderInformationController(UniversityDbContext database, ILogger<FounderInformationController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int id = 0, bool confirmDelete = false)
    {
        if (!ModelState.IsValid || id < 0 || confirmDelete) return NotFound();
        try
        {
            var item = await database.FounderTables.AsNoTracking().SingleOrDefaultAsync();
            if (id > 0 && item?.FounderId != id) return NotFound();
            return View("~/Views/Admin/FounderInformation.cshtml", item is null ? new FounderInformationViewModel() : new FounderInformationViewModel
            {
                Id = item.FounderId, FounderName = item.FounderName,
                Description = item.Description
            });
        }
        catch (Exception exception) { return Unavailable(exception, new FounderInformationViewModel()); }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(FounderInformationViewModel model)
    {
        if (model.Id <= 0) return NotFound();
        try
        {
            await using var transaction = await database.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var item = await database.FounderTables.SingleOrDefaultAsync();
            if (item is null || item.FounderId != model.Id) return NotFound();
            if (!ModelState.IsValid) return View("~/Views/Admin/FounderInformation.cshtml", model);
            item.FounderName = model.FounderName!.Trim();
            item.Description = Clean(model.Description);
            await database.SaveChangesAsync();
            await transaction.CommitAsync();
            TempData["StatusMessage"] = "Founder information saved.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception exception) { return Unavailable(exception, model); }
    }

    private IActionResult Unavailable(Exception exception, FounderInformationViewModel model)
    {
        logger.LogError(exception, "Founder information could not be loaded or saved.");
        Response.StatusCode = 503;
        ViewData["Unavailable"] = true;
        ModelState.AddModelError("", "Founder information is temporarily unavailable. Your changes were not confirmed. Please try again.");
        return View("~/Views/Admin/FounderInformation.cshtml", model);
    }
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
