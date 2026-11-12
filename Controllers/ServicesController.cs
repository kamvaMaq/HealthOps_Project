using Microsoft.AspNetCore.Mvc;

namespace HealthOps_Project.Controllers
{
    public class ServicesController : Controller
    {
        // 🏥 Radiology
        public IActionResult Radiology()
        {
            return View();
        }

        // 🦷 Dentistry
        public IActionResult Dentistry()
        {
            return View();
        }

        // 💊 Pharmacy
        public IActionResult Pharmacy()
        {
            return View();
        }

        // 🧘 Physiotherapy
        public IActionResult Physiotherapy()
        {
            return View();
        }

        // ❤️ Cardiology
        public IActionResult Cardiology()
        {
            return View();
        }

        // 👶 Maternity
        public IActionResult Maternity()
        {
            return View();
        }

        // 👩‍⚕️ General Consultation
        public IActionResult Consultation()
        {
            return View();
        }

        // 🧬 Laboratory
        public IActionResult Laboratory()
        {
            return View();
        }

        // 🩺 Default Index (if you visit /Services)
        public IActionResult Index()
        {
            return View();
        }
    }
}
