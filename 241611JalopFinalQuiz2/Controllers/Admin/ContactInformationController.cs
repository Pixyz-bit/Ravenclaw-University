using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _241611JalopFinalQuiz2.Data;
using _241611JalopFinalQuiz2.Models;
using _241611JalopFinalQuiz2.ViewModels;

namespace _241611JalopFinalQuiz2.Controllers;

[Authorize(Roles = "Admin")]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class ContactInformationController(UniversityDbContext database, ILogger<ContactInformationController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var item = await database.ContactsTables.AsNoTracking().OrderBy(x => x.ContactId).FirstOrDefaultAsync();

            var model = item is null ? new ContactsViewModel() : new ContactsViewModel
            {
                Id = item.ContactId,
                AcademyAddress = item.AcademyAddress,
                Email = item.Email,
                Telephone = item.Telephone,
                OfficeHours = item.OfficeHours,
                TimeIn = item.TimeIn,
                TimeOut = item.TimeOut
            };

            return View("~/Views/Admin/ContactInformation.cshtml", model);
        }
        catch (Exception exception) { return Unavailable(exception, new ContactsViewModel()); }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactsViewModel model)
    {
        if (model.Id < 0) return NotFound();
        try
        {
            model.OfficeHours = await database.ContactsTables.AsNoTracking().OrderBy(x => x.ContactId)
                .Select(x => x.OfficeHours).FirstOrDefaultAsync();
            if (!ModelState.IsValid)
            {

                return View("~/Views/Admin/ContactInformation.cshtml", model);
            }
            await using var transaction = await database.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            var item = await database.ContactsTables.OrderBy(x => x.ContactId).FirstOrDefaultAsync();
            if (item is null) { item = new ContactsTable(); database.ContactsTables.Add(item); }
            item.AcademyAddress = Clean(model.AcademyAddress);
            item.Email = Clean(model.Email);
            item.Telephone = Clean(model.Telephone);
            item.TimeIn = model.TimeIn;
            item.TimeOut = model.TimeOut;

            await database.SaveChangesAsync();
            await transaction.CommitAsync();
            TempData["StatusMessage"] = "Contact information saved.";
            return RedirectToAction(nameof(Index), new { id = (int?)null });
        }
        catch (Exception exception) { return Unavailable(exception, model); }
    }

    private IActionResult Unavailable(Exception exception, ContactsViewModel model)
    {
        logger.LogError(exception, "Contact information could not be loaded or saved.");
        Response.StatusCode = 503;
        ViewData["Unavailable"] = true;
        ModelState.AddModelError("", "This operation is temporarily unavailable. Your changes were not confirmed. Please try again.");
        return View("~/Views/Admin/ContactInformation.cshtml", model);
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
