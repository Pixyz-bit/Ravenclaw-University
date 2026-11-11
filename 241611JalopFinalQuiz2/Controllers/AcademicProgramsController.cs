using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _241611JalopFinalQuiz2.Data;
using _241611JalopFinalQuiz2.Models;
using _241611JalopFinalQuiz2.ViewModels;

namespace _241611JalopFinalQuiz2.Controllers;

[Authorize(Roles = "Admin")]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class AcademicProgramsController(UniversityDbContext database, ILogger<AcademicProgramsController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int id = 0, bool confirmDelete = false)
    {
        if (!ModelState.IsValid || id < 0 || (confirmDelete && id == 0)) return NotFound();
        try
        {
            var item = id == 0 ? null : await database.ProgramsTables.AsNoTracking().SingleOrDefaultAsync(x => x.SubjectId == id);
            if (id > 0 && item is null) return NotFound();
            var model = item is null ? new AcademicProgramsViewModel() : new AcademicProgramsViewModel
            {
                Id = item.SubjectId,
                SubjectName = item.SubjectName,
                Description = item.Description,
                ImagePath = item.ImagePath
            };
            model.ConfirmDelete = confirmDelete;
            model.Items = await database.ProgramsTables.AsNoTracking().OrderBy(x => x.SubjectId).ToListAsync();
            return View(model);
        }
        catch (Exception exception) { return Unavailable(exception, new AcademicProgramsViewModel()); }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(AcademicProgramsViewModel model)
    {
        if (model.Id < 0) return NotFound();
        try
        {
            if (!ModelState.IsValid)
            {
                model.Items = await database.ProgramsTables.AsNoTracking().OrderBy(x => x.SubjectId).ToListAsync();
                return View(model);
            }

            var item = model.Id == 0 ? new ProgramsTable() : await database.ProgramsTables.FindAsync(model.Id);
            if (item is null) return NotFound();
            item.SubjectName = model.SubjectName!.Trim();
            item.Description = Clean(model.Description);
            item.ImagePath = Clean(model.ImagePath);
            if (model.Id == 0) database.ProgramsTables.Add(item);
            await database.SaveChangesAsync();

            TempData["StatusMessage"] = "Academic programs saved.";
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
            var item = await database.ProgramsTables.FindAsync(id);
            if (item is null) return NotFound();
            database.ProgramsTables.Remove(item);
            await database.SaveChangesAsync();
            TempData["StatusMessage"] = "Academic programs deleted.";
            return RedirectToAction(nameof(Index), new { id = (int?)null });
        }
        catch (Exception exception) { return Unavailable(exception, new AcademicProgramsViewModel { Id = id }); }
    }

    private IActionResult Unavailable(Exception exception, AcademicProgramsViewModel model)
    {
        logger.LogError(exception, "Academic programs could not be loaded or saved.");
        Response.StatusCode = 503;
        ViewData["Unavailable"] = true;
        ModelState.AddModelError("", "This operation is temporarily unavailable. Your changes were not confirmed. Please try again.");
        return View("Index", model);
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
