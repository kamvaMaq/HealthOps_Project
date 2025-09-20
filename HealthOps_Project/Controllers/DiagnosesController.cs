using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthOps_Project.Data;
using HealthOps_Project.Models;

namespace HealthOps_Project.Controllers
{
    public class DiagnosesController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DiagnosesController(ApplicationDbContext db) { _db = db; }

        public async Task<IActionResult> Index() => View(await _db.Set<Diagnosis>().ToListAsync());
        public IActionResult Create() => View();
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Diagnosis model){ if(!ModelState.IsValid) return View(model); _db.Add(model); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        public async Task<IActionResult> Edit(int id){ var m = await _db.Set<Diagnosis>().FindAsync(id); if(m==null) return NotFound(); return View(m); }
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Diagnosis model){ if(!ModelState.IsValid) return View(model); _db.Update(model); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        public async Task<IActionResult> Delete(int id){ var m = await _db.Set<Diagnosis>().FindAsync(id); if(m==null) return NotFound(); return View(m); }
        [HttpPost, ActionName("Delete")][ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id){ var m = await _db.Set<Diagnosis>().FindAsync(id); if(m!=null){ _db.Remove(m); await _db.SaveChangesAsync(); } return RedirectToAction(nameof(Index)); }
        public async Task<IActionResult> Details(int id){ var m = await _db.Set<Diagnosis>().FindAsync(id); if(m==null) return NotFound(); return View(m); }
    }
}
