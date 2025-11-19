using System.Diagnostics;
using example_web_app.Models;
using Microsoft.AspNetCore.Mvc;

namespace example_web_app.Controllers
{
    // Main controller for handling home page requests
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // Constructor with dependency injection for logger
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Returns the main landing page view
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
