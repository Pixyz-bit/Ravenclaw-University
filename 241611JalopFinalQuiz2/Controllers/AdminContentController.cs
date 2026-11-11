using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _241611JalopFinalQuiz2.Services;
using _241611JalopFinalQuiz2.ViewModels;

namespace _241611JalopFinalQuiz2.Controllers;

[Authorize(Roles = "Admin")]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class AdminContentController(ContentEditorService editor, ILogger<AdminContentController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(ContentSection section)
    {
        if (!ModelState.IsValid || !Enum.IsDefined(section)) return NotFound();
        try { return View(new ContentListViewModel { Section = section, Items = await editor.ListAsync(section) }); }
        catch (Exception exception)
        {
            logger.LogError(exception, "Content list could not be loaded.");
            Response.StatusCode = 503;
            return View(new ContentListViewModel { Section = section, Unavailable = true });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(ContentSection section, int id = 0)
    {
        if (!ModelState.IsValid || !Enum.IsDefined(section) || id < 0) return NotFound();
        try
        {
            var items = await editor.ListAsync(section);
            if (id == 0 && ContentSections.IsSingleton(section) && items.Count > 0)
                return RedirectToAction(nameof(Edit), new { section, id = items[0].Id });
            var model = id == 0 ? new ContentEditorViewModel { Section = section } : items.SingleOrDefault(x => x.Id == id);
            return model is null ? NotFound() : View(model);
        }
        catch (Exception exception) { return EditorError(exception, new ContentEditorViewModel { Section = section, Id = id }); }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ContentEditorViewModel model)
    {
        if (!Enum.IsDefined(model.Section) || model.Id < 0) return NotFound();
        if (!ModelState.IsValid) return View(model);
        try
        {
            var result = await editor.SaveAsync(model);
            if (result == ContentSaveResult.Missing) return NotFound();
            if (result == ContentSaveResult.SingletonExists)
            {
                ModelState.AddModelError("", "A record already exists. Return to the list and edit it.");
                return View(model);
            }
            TempData["StatusMessage"] = "Content saved. Changes are now available on the university page.";
            return RedirectToAction(nameof(Index), new { section = model.Section });
        }
        catch (Exception exception) { return EditorError(exception, model); }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(ContentSection section, int id)
    {
        if (!ModelState.IsValid || !Enum.IsDefined(section) || ContentSections.IsSingleton(section) || id <= 0) return NotFound();
        try
        {
            var item = (await editor.ListAsync(section)).SingleOrDefault(x => x.Id == id);
            return item is null ? NotFound() : View(item);
        }
        catch (Exception exception) { return EditorError(exception, new ContentEditorViewModel { Section = section, Id = id }); }
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(ContentSection section, int id)
    {
        if (!ModelState.IsValid || !Enum.IsDefined(section) || ContentSections.IsSingleton(section) || id <= 0) return NotFound();
        try
        {
            if (!await editor.DeleteAsync(section, id)) return NotFound();
            TempData["StatusMessage"] = "Content deleted.";
            return RedirectToAction(nameof(Index), new { section });
        }
        catch (Exception exception) { return EditorError(exception, new ContentEditorViewModel { Section = section, Id = id }); }
    }

    private IActionResult EditorError(Exception exception, ContentEditorViewModel model)
    {
        logger.LogError(exception, "Content operation could not be completed.");
        Response.StatusCode = 503;
        ModelState.AddModelError("", "This operation is temporarily unavailable. Your changes were not confirmed. Try again or return to the list.");
        return View("Edit", model);
    }
}
