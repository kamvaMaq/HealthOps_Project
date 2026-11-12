using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [Authorize(Roles = "WardAdmin")]
        public IActionResult WardAdmin()
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

        [Authorize(Roles = "NursingSister")]
        public IActionResult NursingSister()
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

        public IActionResult Index()
        {
            return View();
        }


        [Authorize]
        public IActionResult BackToDashboard()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Admin", "Dashboard");
            if (User.IsInRole("WardAdmin"))
                return RedirectToAction("WardAdmin", "Dashboard");

            if (User.IsInRole("Doctor"))
                return RedirectToAction("Doctor", "Dashboard");

            if (User.IsInRole("Nurse"))
                return RedirectToAction("Nurse", "Dashboard");

            if (User.IsInRole("NursingSister"))
                return RedirectToAction("NursingSister", "Dashboard");

            if (User.IsInRole("ScriptManager"))
                return RedirectToAction("ScriptManager", "Dashboard");

            if (User.IsInRole("StockManager"))
                return RedirectToAction("StockManager", "Dashboard");

            // fallback
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
