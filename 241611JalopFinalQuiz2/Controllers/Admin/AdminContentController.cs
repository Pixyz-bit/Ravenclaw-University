using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _241611JalopFinalQuiz2.ViewModels;

namespace _241611JalopFinalQuiz2.Controllers;

// Compatibility links only. Each section owns its controller, form model, and view.
[Authorize(Roles = "Admin")]
public class AdminContentController : Controller
{
    [HttpGet]
    public IActionResult Index(ContentSection section) => RedirectSection(section);

    [HttpGet]
    public IActionResult Edit(ContentSection section, int id = 0) => RedirectSection(section, id);

    [HttpGet]
    public IActionResult Delete(ContentSection section, int id) => ContentSections.IsSingleton(section)
        ? NotFound() : RedirectSection(section, id, true);

    private IActionResult RedirectSection(ContentSection section, int id = 0, bool confirmDelete = false)
    {
        if (!ModelState.IsValid || !Enum.IsDefined(section) || id < 0) return NotFound();
        if (ContentSections.IsSingleton(section) || id == 0)
            return RedirectToAction("Index", ContentSections.ControllerName(section));
        return RedirectToAction("Index", ContentSections.ControllerName(section), new { id, confirmDelete });
    }
}
