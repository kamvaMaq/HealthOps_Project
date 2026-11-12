using Microsoft.AspNetCore.Mvc;
using HealthOps_Project.Models;
using HealthOps_Project.Services;
using Microsoft.AspNetCore.Authorization;

namespace HealthOps_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOpenAIService _openAIService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IOpenAIService openAIService, ILogger<HomeController> logger)
        {
            _openAIService = openAIService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid chat request received");
                return Json(new ChatResponse
                {
                    Success = false,
                    Error = "Invalid request format",
                    Reply = "Please provide a valid message."
                });
            }

            try
            {
                _logger.LogInformation("Processing chat request: {Message}", request.Message);
                var response = await _openAIService.GetHealthOPSResponseAsync(request.Message);
                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in HomeController.Chat");
                return Json(new ChatResponse
                {
                    Success = false,
                    Error = "Internal server error",
                    Reply = "I'm experiencing technical difficulties. Please try again later."
                });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}