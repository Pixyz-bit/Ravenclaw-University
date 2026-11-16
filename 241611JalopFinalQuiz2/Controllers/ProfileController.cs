using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _241611JalopFinalQuiz2.Data;
using _241611JalopFinalQuiz2.Services;
using _241611JalopFinalQuiz2.ViewModels;

namespace _241611JalopFinalQuiz2.Controllers;

[Authorize]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class ProfileController(UniversityDbContext database, ImageUploadService images, ILogger<ProfileController> logger) : Controller
{
    private int CurrentId => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
    private Task<ProfileViewModel?> ReadProfile() => database.UserTables.AsNoTracking()
        .Where(user => user.UserId == CurrentId)
        .Select(user => new ProfileViewModel { Username = user.Username, FirstName = user.FirstName,
            MiddleName = user.MiddleName, LastName = user.LastName, Birthday = user.Birthday, ProfileImg = user.ProfileImg })
        .SingleOrDefaultAsync();

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var model = await ReadProfile();
            return model is null ? NotFound() : View(model);
        }
        catch (Exception exception) { return Unavailable(exception, new ProfileViewModel()); }
    }

    [HttpPost, ValidateAntiForgeryToken, RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> Index(ProfileViewModel model)
    {
        string? uploadedPath = null;
        try
        {
            var existing = await ReadProfile();
            if (existing is null) return NotFound();
            model.Username = existing.Username; model.ProfileImg = existing.ProfileImg;
            var imageError = await images.ValidateAsync(model.ImageUpload);
            if (imageError is not null) ModelState.AddModelError(nameof(model.ImageUpload), imageError);
            if (!ModelState.IsValid) return View(model);
            if (model.ImageUpload is not null) uploadedPath = await images.SaveAsync(model.ImageUpload);
            var profilePath = uploadedPath ?? existing.ProfileImg;
            var firstName = model.FirstName!.Trim(); var lastName = model.LastName!.Trim();
            var middleName = string.IsNullOrWhiteSpace(model.MiddleName) ? null : model.MiddleName.Trim();
            var role = User.FindFirstValue(ClaimTypes.Role);
            // Ownership comes only from the authenticated cookie; no posted account ID is accepted.
            var changed = await database.UserTables.Where(user => user.UserId == CurrentId && user.IsActive
                    && user.Username == existing.Username && user.Role == role)
                .ExecuteUpdateAsync(setters => setters.SetProperty(user => user.FirstName, firstName)
                    .SetProperty(user => user.LastName, lastName).SetProperty(user => user.MiddleName, middleName)
                    .SetProperty(user => user.Birthday, model.Birthday).SetProperty(user => user.ProfileImg, profilePath));
            if (changed != 1) { images.Discard(uploadedPath); return Forbid(); }
            uploadedPath = null;
            TempData["StatusMessage"] = "Your personal information has been saved.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception exception) { images.Discard(uploadedPath); return Unavailable(exception, model); }
    }

    private IActionResult Unavailable(Exception exception, ProfileViewModel model)
    {
        logger.LogError(exception, "Personal information could not be loaded or saved.");
        Response.StatusCode = 503; ViewData["Unavailable"] = true;
        ModelState.AddModelError("", "Your personal information is temporarily unavailable. Changes were not confirmed. Please try again.");
        return View("Index", model);
    }
}
