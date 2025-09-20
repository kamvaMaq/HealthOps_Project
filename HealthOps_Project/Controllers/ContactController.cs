using Microsoft.AspNetCore.Mvc;
using HealthOps_Project.Data;
using HealthOps_Project.Models;

namespace HealthOps_Project.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ContactController(ApplicationDbContext db) { _db = db; }
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Index(ContactMessage model)
        {
            if (!ModelState.IsValid) return View(model);
            _db.ContactMessages.Add(model);
            await _db.SaveChangesAsync();
            TempData["Message"] = "Thank you, we'll get back to you.";
            return RedirectToAction("Index");
        }
    }
}
