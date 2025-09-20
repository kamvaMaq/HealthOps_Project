using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthOps_Project.Data;
using HealthOps_Project.Models;

namespace HealthOps_Project.Controllers
{
    public class ScriptsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ScriptsController(ApplicationDbContext db) { _db = db; }
        public async Task<IActionResult> Index() => View(await _db.Scripts.ToListAsync());
        public IActionResult Create() => View();
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Script script){ if (!ModelState.IsValid) return View(script); _db.Scripts.Add(script); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        public async Task<IActionResult> Edit(int id){ var s = await _db.Scripts.FindAsync(id); if (s==null) return NotFound(); return View(s); }
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Script script){ if (!ModelState.IsValid) return View(script); _db.Scripts.Update(script); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        public async Task<IActionResult> Delete(int id){ var s = await _db.Scripts.FindAsync(id); if (s==null) return NotFound(); return View(s); }
        [HttpPost, ActionName("Delete")][ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id){ var s = await _db.Scripts.FindAsync(id); if (s!=null){ _db.Scripts.Remove(s); await _db.SaveChangesAsync(); } return RedirectToAction(nameof(Index)); }
    }
}
