using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _241611JalopFinalQuiz2.Data;
using _241611JalopFinalQuiz2.ViewModels;

namespace _241611JalopFinalQuiz2.Controllers;

[Authorize(Roles = "Admin")]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class AdminController(UniversityDbContext database, ILogger<AdminController> logger) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        try
        {
            var model = new AdminDashboardViewModel
            {
                Programs = await database.ProgramsTables.CountAsync(cancellationToken),
                CoreValues = await database.CoreValuesTables.CountAsync(cancellationToken),
                GalleryImages = await database.AboutImagesTables.CountAsync(cancellationToken)
            };
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
