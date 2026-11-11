using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _241611JalopFinalQuiz2.Data;
using _241611JalopFinalQuiz2.Models;
using _241611JalopFinalQuiz2.ViewModels;

namespace _241611JalopFinalQuiz2.Controllers;

[Authorize(Roles = "Admin")]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class AboutGalleryController(UniversityDbContext database, ILogger<AboutGalleryController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int id = 0, bool confirmDelete = false)
    {
        if (!ModelState.IsValid || id < 0 || (confirmDelete && id == 0)) return NotFound();
        try
        {
            var item = id == 0 ? null : await database.AboutImagesTables.AsNoTracking().SingleOrDefaultAsync(x => x.AboutImageId == id);
            if (id > 0 && item is null) return NotFound();
            var model = item is null ? new AboutGalleryViewModel() : new AboutGalleryViewModel
            {
                Id = item.AboutImageId,
                ImagePath = item.ImagePath,
                AltText = item.AltText
            };
            model.ConfirmDelete = confirmDelete;
            model.Items = await database.AboutImagesTables.AsNoTracking().OrderBy(x => x.AboutImageId).ToListAsync();
            return View(model);
        }
        catch (Exception exception) { return Unavailable(exception, new AboutGalleryViewModel()); }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(AboutGalleryViewModel model)
    {
        if (model.Id < 0) return NotFound();
        try
        {
            if (!ModelState.IsValid)
            {
                model.Items = await database.AboutImagesTables.AsNoTracking().OrderBy(x => x.AboutImageId).ToListAsync();
                return View(model);
            }

            var item = model.Id == 0 ? new AboutImagesTable() : await database.AboutImagesTables.FindAsync(model.Id);
            if (item is null) return NotFound();
            item.ImagePath = model.ImagePath!.Trim();
            item.AltText = Clean(model.AltText);
            if (model.Id == 0) database.AboutImagesTables.Add(item);
            await database.SaveChangesAsync();

            TempData["StatusMessage"] = "About gallery saved.";
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
            var item = await database.AboutImagesTables.FindAsync(id);
            if (item is null) return NotFound();
            database.AboutImagesTables.Remove(item);
            await database.SaveChangesAsync();
            TempData["StatusMessage"] = "About gallery deleted.";
            return RedirectToAction(nameof(Index), new { id = (int?)null });
        }
        catch (Exception exception) { return Unavailable(exception, new AboutGalleryViewModel { Id = id }); }
    }

    private IActionResult Unavailable(Exception exception, AboutGalleryViewModel model)
    {
        logger.LogError(exception, "About gallery could not be loaded or saved.");
        Response.StatusCode = 503;
        ViewData["Unavailable"] = true;
        ModelState.AddModelError("", "This operation is temporarily unavailable. Your changes were not confirmed. Please try again.");
        return View("Index", model);
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
