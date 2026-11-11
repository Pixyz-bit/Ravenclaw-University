using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _241611JalopFinalQuiz2.Data;
using _241611JalopFinalQuiz2.Models;
using _241611JalopFinalQuiz2.ViewModels;

namespace _241611JalopFinalQuiz2.Controllers;

[Authorize(Roles = "Admin")]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class SocialLinksController(UniversityDbContext database, ILogger<SocialLinksController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int id = 0, bool confirmDelete = false)
    {
        if (!ModelState.IsValid || id < 0 || (confirmDelete && id == 0)) return NotFound();
        try
        {
            var item = id == 0 ? null : await database.SocialLinksTables.AsNoTracking().SingleOrDefaultAsync(x => x.SocialLinkId == id);
            if (id > 0 && item is null) return NotFound();
            var model = item is null ? new SocialLinksViewModel() : new SocialLinksViewModel
            {
                Id = item.SocialLinkId,
                PlatformName = item.PlatformName,
                Url = item.Url,
                IconClass = item.IconClass
            };
            model.ConfirmDelete = confirmDelete;
            model.Items = await database.SocialLinksTables.AsNoTracking().OrderBy(x => x.SocialLinkId).ToListAsync();
            return View(model);
        }
        catch (Exception exception) { return Unavailable(exception, new SocialLinksViewModel()); }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SocialLinksViewModel model)
    {
        if (model.Id < 0) return NotFound();
        try
        {
            if (!ModelState.IsValid)
            {
                model.Items = await database.SocialLinksTables.AsNoTracking().OrderBy(x => x.SocialLinkId).ToListAsync();
                return View(model);
            }

            var item = model.Id == 0 ? new SocialLinksTable() : await database.SocialLinksTables.FindAsync(model.Id);
            if (item is null) return NotFound();
            item.PlatformName = model.PlatformName!.Trim();
            item.Url = model.Url!.Trim();
            item.IconClass = Clean(model.IconClass);
            if (model.Id == 0) database.SocialLinksTables.Add(item);
            await database.SaveChangesAsync();

            TempData["StatusMessage"] = "Social links saved.";
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
            var item = await database.SocialLinksTables.FindAsync(id);
            if (item is null) return NotFound();
            database.SocialLinksTables.Remove(item);
            await database.SaveChangesAsync();
            TempData["StatusMessage"] = "Social links deleted.";
            return RedirectToAction(nameof(Index), new { id = (int?)null });
        }
        catch (Exception exception) { return Unavailable(exception, new SocialLinksViewModel { Id = id }); }
    }

    private IActionResult Unavailable(Exception exception, SocialLinksViewModel model)
    {
        logger.LogError(exception, "Social links could not be loaded or saved.");
        Response.StatusCode = 503;
        ViewData["Unavailable"] = true;
        ModelState.AddModelError("", "This operation is temporarily unavailable. Your changes were not confirmed. Please try again.");
        return View("Index", model);
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
