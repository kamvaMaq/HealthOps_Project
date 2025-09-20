using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthOps_Project.Data;
using HealthOps_Project.Models;

namespace HealthOps_Project.Controllers
{
    public class WardsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public WardsController(ApplicationDbContext db) { _db = db; }
        public async Task<IActionResult> Index() => View(await _db.Wards.ToListAsync());
        public IActionResult Create() => View();
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ward ward){ if (!ModelState.IsValid) return View(ward); _db.Wards.Add(ward); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        public async Task<IActionResult> Edit(int id){ var w = await _db.Wards.FindAsync(id); if (w==null) return NotFound(); return View(w); }
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Ward ward){ if (!ModelState.IsValid) return View(ward); _db.Wards.Update(ward); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        public async Task<IActionResult> Delete(int id){ var w = await _db.Wards.FindAsync(id); if (w==null) return NotFound(); return View(w); }
        [HttpPost, ActionName("Delete")][ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id){ var w = await _db.Wards.FindAsync(id); if (w!=null){ _db.Wards.Remove(w); await _db.SaveChangesAsync(); } return RedirectToAction(nameof(Index)); }
    }
}
