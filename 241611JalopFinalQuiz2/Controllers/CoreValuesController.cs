using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _241611JalopFinalQuiz2.Data;
using _241611JalopFinalQuiz2.Models;
using _241611JalopFinalQuiz2.ViewModels;

namespace _241611JalopFinalQuiz2.Controllers;

[Authorize(Roles = "Admin")]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class CoreValuesController(UniversityDbContext database, ILogger<CoreValuesController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int id = 0, bool confirmDelete = false)
    {
        if (!ModelState.IsValid || id < 0 || (confirmDelete && id == 0)) return NotFound();
        try
        {
            var item = id == 0 ? null : await database.CoreValuesTables.AsNoTracking().SingleOrDefaultAsync(x => x.CoreValueId == id);
            if (id > 0 && item is null) return NotFound();
            var model = item is null ? new CoreValuesViewModel() : new CoreValuesViewModel
            {
                Id = item.CoreValueId,
                Title = item.Title
            };
            model.ConfirmDelete = confirmDelete;
            model.Items = await database.CoreValuesTables.AsNoTracking().OrderBy(x => x.CoreValueId).ToListAsync();
            return View(model);
        }
        catch (Exception exception) { return Unavailable(exception, new CoreValuesViewModel()); }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CoreValuesViewModel model)
    {
        if (model.Id < 0) return NotFound();
        try
        {
            if (!ModelState.IsValid)
            {
                model.Items = await database.CoreValuesTables.AsNoTracking().OrderBy(x => x.CoreValueId).ToListAsync();
                return View(model);
            }

            var item = model.Id == 0 ? new CoreValuesTable() : await database.CoreValuesTables.FindAsync(model.Id);
            if (item is null) return NotFound();
            item.Title = model.Title!.Trim();
            if (model.Id == 0) database.CoreValuesTables.Add(item);
            await database.SaveChangesAsync();

            TempData["StatusMessage"] = "Core values saved.";
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
            var item = await database.CoreValuesTables.FindAsync(id);
            if (item is null) return NotFound();
            database.CoreValuesTables.Remove(item);
            await database.SaveChangesAsync();
            TempData["StatusMessage"] = "Core values deleted.";
            return RedirectToAction(nameof(Index), new { id = (int?)null });
        }
        catch (Exception exception) { return Unavailable(exception, new CoreValuesViewModel { Id = id }); }
    }

    private IActionResult Unavailable(Exception exception, CoreValuesViewModel model)
    {
        logger.LogError(exception, "Core values could not be loaded or saved.");
        Response.StatusCode = 503;
        ViewData["Unavailable"] = true;
        ModelState.AddModelError("", "This operation is temporarily unavailable. Your changes were not confirmed. Please try again.");
        return View("Index", model);
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
