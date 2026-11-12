using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _241611JalopFinalQuiz2.Data;
using _241611JalopFinalQuiz2.Services;
using _241611JalopFinalQuiz2.ViewModels;

namespace _241611JalopFinalQuiz2.Controllers;

[Authorize(Roles = "Admin")]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class AccountManagementController(UniversityDbContext database, ImageUploadService images, ILogger<AccountManagementController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int id = 0, string? search = null)
    {
        if (!ModelState.IsValid || id < 0 || search?.Length > 100) return BadRequest();
        try
        {
            var model = id == 0 ? new AccountManagementViewModel() : await Summaries().SingleOrDefaultAsync(x => x.Id == id);
            if (model is null) return NotFound();
            model.Search = search;
            await LoadAccounts(model);
            return View(model);
        }
        catch (Exception exception) { return Unavailable(exception, new AccountManagementViewModel()); }
    }

    [HttpPost, ValidateAntiForgeryToken, RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> Index(AccountManagementViewModel model)
    {
        if (model.Id <= 0) return NotFound();
        string? uploadedPath = null;
        try
        {
            await using var transaction = await database.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var account = await database.UserTables.FindAsync(model.Id);
            if (account is null) return NotFound();
            model.ProfileImg = account.ProfileImg;
            var imageError = await images.ValidateAsync(model.ImageUpload);
            if (imageError is not null) ModelState.AddModelError(nameof(model.ImageUpload), imageError);
            if (User.FindFirstValue(ClaimTypes.NameIdentifier) == model.Id.ToString() && (!model.IsActive || model.Role != "Admin"))
                ModelState.AddModelError("", "You cannot deactivate your own account or remove your own Admin role.");
            if (account.IsActive && account.Role == "Admin" && (!model.IsActive || model.Role != "Admin")
                && !await database.UserTables.AnyAsync(x => x.UserId != model.Id && x.IsActive && x.Role == "Admin"))
                ModelState.AddModelError("", "Keep at least one active Admin account.");
            if (!string.IsNullOrWhiteSpace(model.Username) && await database.UserTables.AnyAsync(x => x.UserId != model.Id
                && EF.Functions.Collate(x.Username, "Latin1_General_100_CI_AS") == model.Username.Trim()))
                ModelState.AddModelError(nameof(model.Username), "This username is already in use.");
            if (!ModelState.IsValid) { await LoadAccounts(model); return View(model); }
            account.Username = model.Username!.Trim();
            account.FirstName = Clean(model.FirstName); account.MiddleName = Clean(model.MiddleName); account.LastName = Clean(model.LastName);
            account.Birthday = model.Birthday; account.Role = model.Role; account.IsActive = model.IsActive;
            if (model.ImageUpload is not null)
            {
                uploadedPath = await images.SaveAsync(model.ImageUpload);
                account.ProfileImg = uploadedPath;
            }
            await database.SaveChangesAsync();
            await transaction.CommitAsync();
            uploadedPath = null;
            TempData["StatusMessage"] = "Account updated.";
            return RedirectToAction(nameof(Index), new { id = (int?)null, search = model.Search });
        }
        catch (Exception exception) { images.Discard(uploadedPath); return Unavailable(exception, model); }
    }

    // Project only fields needed on this screen; password hashes never enter the view.
    private IQueryable<AccountManagementViewModel> Summaries() => database.UserTables.AsNoTracking().Select(x => new AccountManagementViewModel
    {
        Id = x.UserId, Username = x.Username, Role = x.Role, IsActive = x.IsActive,
        FirstName = x.FirstName, MiddleName = x.MiddleName, LastName = x.LastName,
        Birthday = x.Birthday, ProfileImg = x.ProfileImg
    });

    private async Task LoadAccounts(AccountManagementViewModel model)
    {
        var query = Summaries();
        if (!string.IsNullOrWhiteSpace(model.Search))
        {
            var search = model.Search.Trim();
            query = query.Where(x => x.Username!.Contains(search) || (x.FirstName != null && x.FirstName.Contains(search)) || (x.LastName != null && x.LastName.Contains(search)));
        }
        model.Items = await query.OrderBy(x => x.Username).Take(100).ToListAsync();
    }

    private IActionResult Unavailable(Exception exception, AccountManagementViewModel model)
    {
        logger.LogError(exception, "Account management could not be loaded or saved.");
        Response.StatusCode = 503;
        ViewData["Unavailable"] = true;
        ModelState.AddModelError("", "Account management is temporarily unavailable. Your changes were not confirmed. Please try again.");
        return View("Index", model);
    }
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
