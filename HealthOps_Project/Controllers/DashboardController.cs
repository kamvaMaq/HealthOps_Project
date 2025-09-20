using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthOps_Project.Controllers
{
    [Authorize] // all dashboard pages require login
    public class DashboardController : Controller
    {
        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return View();
        }

        [Authorize(Roles = "Doctor")]
        public IActionResult Doctor()
        {
            return View();
        }

        [Authorize(Roles = "Nurse")]
        public IActionResult Nurse()
        {
            return View();
        }

        [Authorize(Roles = "ScriptManager")]
        public IActionResult ScriptManager()
        {
            return View();
        }

        [Authorize(Roles = "StockManager")]
        public IActionResult StockManager()
        {
            return View();
        }
    }
}
