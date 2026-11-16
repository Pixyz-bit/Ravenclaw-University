using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _241611JalopFinalQuiz2.Data;
using _241611JalopFinalQuiz2.Services;
using _241611JalopFinalQuiz2.ViewModels;

namespace _241611JalopFinalQuiz2.Controllers;

[Authorize(Roles = "Admin")]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class AdminController(UniversityDbContext database, UniversityContentService content, ILogger<AdminController> logger) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        try
        {
            var page = await content.ReadAsync(cancellationToken);
            var accounts = await database.UserTables.AsNoTracking().GroupBy(x => 1)
                .Select(group => new { Total = group.Count(), Active = group.Count(x => x.IsActive) })
                .SingleOrDefaultAsync(cancellationToken);
            var model = new AdminDashboardViewModel
            {
                Programs = page.Programs.Count,
                CoreValues = page.Values.Count,
                GalleryImages = page.Photos.Count,
                Accounts = accounts?.Total ?? 0,
                ActiveUsers = accounts?.Active ?? 0
            };
            if (string.IsNullOrWhiteSpace(page.University?.AcademyDescription)
                || string.IsNullOrWhiteSpace(page.University?.Mission)
                || string.IsNullOrWhiteSpace(page.University?.Vision))
                model.Reminders.Add(new("Complete the university introduction", "Review the description, mission, and vision.", "UniversityInformation"));
            if (string.IsNullOrWhiteSpace(page.Contact?.AcademyAddress)
                || string.IsNullOrWhiteSpace(page.Contact?.Email)
                || string.IsNullOrWhiteSpace(page.Contact?.Telephone)
                || string.IsNullOrWhiteSpace(page.Contact?.OfficeHours))
                model.Reminders.Add(new("Complete your contact details", "Make the address, email, telephone, and office hours easy to find.", "ContactInformation"));
            if (page.Programs.Count == 0)
                model.Reminders.Add(new("Add your first academic program", "Introduce the subjects students can explore.", "AcademicPrograms"));
            else if (page.Programs.Any(program => string.IsNullOrWhiteSpace(program.ImagePath) || string.IsNullOrWhiteSpace(program.Description)))
                model.Reminders.Add(new("Finish your program entries", "Some programs need an image or description.", "AcademicPrograms"));
            if (page.Values.Count == 0)
                model.Reminders.Add(new("Add the university's core values", "Share the principles that guide the university.", "CoreValues"));
            if (page.Founders.Count == 0)
                model.Reminders.Add(new("Introduce university leadership", "Add a founder name and biography.", "FounderInformation"));
            return View(model);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Administration overview could not be loaded.");
            Response.StatusCode = 503;
            return View(new AdminDashboardViewModel { Unavailable = true });
        }
    }
}
