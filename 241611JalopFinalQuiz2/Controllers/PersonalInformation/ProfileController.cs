using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _241611JalopFinalQuiz2.Data;
using _241611JalopFinalQuiz2.Models;
using _241611JalopFinalQuiz2.Services;
using _241611JalopFinalQuiz2.ViewModels;

namespace _241611JalopFinalQuiz2.Controllers;

[Authorize]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class ProfileController(UniversityDbContext database, ImageUploadService images,
    IPasswordHasher<UserTable> hasher, ILogger<ProfileController> logger) : Controller
{
    private int CurrentId => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
    private Task<ProfileViewModel?> ReadProfile() => database.UserTables.AsNoTracking()
        .Where(user => user.UserId == CurrentId)
        .Select(user => new ProfileViewModel { Username = user.Username, FirstName = user.FirstName,
            MiddleName = user.MiddleName, LastName = user.LastName, Birthday = user.Birthday, ProfileImg = user.ProfileImg })
        .SingleOrDefaultAsync();

    [HttpGet]
    public async Task<IActionResult> Index(string? tab)
    {
        ViewData["PasswordTab"] = tab == "password";
        try
        {
            var model = await ReadProfile();
            return model is null ? NotFound() : View("~/Views/User/Profile.cshtml", model);
        }
        catch (Exception exception) { return Unavailable(exception, new ProfileViewModel()); }
    }

    [HttpPost, ValidateAntiForgeryToken, EnableRateLimiting("account-forms")]
    public async Task<IActionResult> ChangePassword([Bind(Prefix = "Password")] ChangePasswordViewModel password)
    {
        ViewData["PasswordTab"] = true;
        ProfileViewModel? profile = null;
        try
        {
            profile = await ReadProfile();
            if (profile is null) return NotFound();
            if (ModelState.IsValid)
            {
                var user = await database.UserTables.AsNoTracking().SingleOrDefaultAsync(user => user.UserId == CurrentId);
                if (user is null) return NotFound();
                if (!user.IsActive || user.Role != User.FindFirstValue(ClaimTypes.Role)
                    || user.Username != User.Identity?.Name) return Forbid();
                PasswordVerificationResult verified;
                try { verified = hasher.VerifyHashedPassword(user, user.PasswordHash, password.CurrentPassword); }
                catch (FormatException) { verified = PasswordVerificationResult.Failed; }
                if (verified == PasswordVerificationResult.Failed)
                    ModelState.AddModelError("Password.CurrentPassword", "Current password is incorrect.");
                else if (password.CurrentPassword == password.NewPassword)
                    ModelState.AddModelError("Password.NewPassword", "Choose a different password.");
                else
                {
                    var originalHash = user.PasswordHash;
                    var newHash = hasher.HashPassword(user, password.NewPassword);
                    // A concurrent password/account change must not be overwritten.
                    var changed = await database.UserTables.Where(account => account.UserId == CurrentId
                            && account.IsActive && account.Username == user.Username && account.Role == user.Role
                            && account.PasswordHash == originalHash)
                        .ExecuteUpdateAsync(setters => setters.SetProperty(account => account.PasswordHash, newHash));
                    if (changed != 1)
                        ModelState.AddModelError("", "Your account changed. Reload the page and try again.");
                    else
                    {
                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        TempData["StatusMessage"] = "Password updated. Sign in with your new password.";
                        return RedirectToAction("Login", "Account");
                    }
                }
            }
            return View("~/Views/User/Profile.cshtml", profile);
        }
        catch (Exception exception) { return Unavailable(exception, profile ?? new ProfileViewModel()); }
        finally
        {
            // Never retain submitted secrets in a rendered model or attempted-value state.
            password.CurrentPassword = password.NewPassword = password.ConfirmPassword = "";
            foreach (var key in ModelState.Keys.Where(key => key.StartsWith("Password.", StringComparison.Ordinal)).ToArray())
                ModelState.SetModelValue(key, null, null);
        }
    }

    [HttpPost, ValidateAntiForgeryToken, RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> Index(ProfileViewModel model)
    {
        string? uploadedPath = null;
        try
        {
            var existing = await ReadProfile();
            if (existing is null) return NotFound();
            ViewData["SavedProfile"] = existing;
            model.Username = existing.Username; model.ProfileImg = existing.ProfileImg;
            var imageError = await images.ValidateAsync(model.ImageUpload);
            if (imageError is not null) ModelState.AddModelError(nameof(model.ImageUpload), imageError);
            if (!ModelState.IsValid) return View("~/Views/User/Profile.cshtml", model);
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
        return View("~/Views/User/Profile.cshtml", model);
    }
}
