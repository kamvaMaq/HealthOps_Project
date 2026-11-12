using Microsoft.AspNetCore.Mvc;

namespace HealthOps_Project.Controllers
{
    public class NurseController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}

