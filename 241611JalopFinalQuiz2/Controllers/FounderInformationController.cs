using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _241611JalopFinalQuiz2.Data;
using _241611JalopFinalQuiz2.Models;
using _241611JalopFinalQuiz2.ViewModels;

namespace _241611JalopFinalQuiz2.Controllers;

[Authorize(Roles = "Admin")]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class FounderInformationController(UniversityDbContext database, ILogger<FounderInformationController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int id = 0, bool confirmDelete = false)
    {
        if (!ModelState.IsValid || id < 0 || (confirmDelete && id == 0)) return NotFound();
        try
        {
            var item = id == 0 ? null : await database.FounderTables.AsNoTracking().SingleOrDefaultAsync(x => x.FounderId == id);
            if (id > 0 && item is null) return NotFound();
            var model = item is null ? new FounderInformationViewModel() : new FounderInformationViewModel
            {
                Id = item.FounderId,
                FounderName = item.FounderName,
                Birthday = item.Birthday,
                Description = item.Description,
                FounderMessage = item.FounderMessage
            };
            model.ConfirmDelete = confirmDelete;
            model.Items = await database.FounderTables.AsNoTracking().OrderBy(x => x.FounderId).ToListAsync();
            return View(model);
        }
        catch (Exception exception) { return Unavailable(exception, new FounderInformationViewModel()); }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(FounderInformationViewModel model)
    {
        if (model.Id < 0) return NotFound();
        try
        {
            if (!ModelState.IsValid)
            {
                model.Items = await database.FounderTables.AsNoTracking().OrderBy(x => x.FounderId).ToListAsync();
                return View(model);
            }

            var item = model.Id == 0 ? new FounderTable() : await database.FounderTables.FindAsync(model.Id);
            if (item is null) return NotFound();
            item.FounderName = model.FounderName!.Trim();
            item.Birthday = model.Birthday;
            item.Description = Clean(model.Description);
            item.FounderMessage = Clean(model.FounderMessage);
            if (model.Id == 0) database.FounderTables.Add(item);
            await database.SaveChangesAsync();

            TempData["StatusMessage"] = "Founder information saved.";
            return RedirectToAction(nameof(Index), new { id = (int?)null });
        }
        catch (Exception exception) { return Unavailable(exception, model); }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!ModelState.IsValid || id <= 0) return NotFound();
        try
        {
            var item = await database.FounderTables.FindAsync(id);
            if (item is null) return NotFound();
            database.FounderTables.Remove(item);
            await database.SaveChangesAsync();
            TempData["StatusMessage"] = "Founder information deleted.";
            return RedirectToAction(nameof(Index), new { id = (int?)null });
        }
        catch (Exception exception) { return Unavailable(exception, new FounderInformationViewModel { Id = id }); }
    }

    private IActionResult Unavailable(Exception exception, FounderInformationViewModel model)
    {
        logger.LogError(exception, "Founder information could not be loaded or saved.");
        Response.StatusCode = 503;
        ViewData["Unavailable"] = true;
        ModelState.AddModelError("", "This operation is temporarily unavailable. Your changes were not confirmed. Please try again.");
        return View("Index", model);
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
