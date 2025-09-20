using Microsoft.AspNetCore.Mvc;

namespace HealthOps_Project.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index() => View();
    }
}
