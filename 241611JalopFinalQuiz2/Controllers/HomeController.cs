using System.Diagnostics;
using _241611JalopFinalQuiz2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using _241611JalopFinalQuiz2.Services;
using _241611JalopFinalQuiz2.ViewModels;

namespace _241611JalopFinalQuiz2.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UniversityContentService _content;

        public HomeController(ILogger<HomeController> logger, UniversityContentService content)
        {
            _logger = logger;
            _content = content;
        }

        public async Task<IActionResult> Index() //loads and displays the homepage
        {
            try { return View("~/Views/User/Home.cshtml", await _content.ReadAsync(HttpContext.RequestAborted)); }
            catch (Exception exception)
            {
                _logger.LogError(exception, "University content could not be loaded.");
                Response.StatusCode = 503;
                return View("~/Views/User/Home.cshtml", new UniversityPageViewModel { Unavailable = true });
            }
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
