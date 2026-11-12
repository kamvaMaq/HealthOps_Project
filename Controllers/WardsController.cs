using HealthOps_Project.Data;
using HealthOps_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthOps_Project.Controllers
{
    public class WardsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WardsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Wards
        public async Task<IActionResult> Index(string? searchTerm)
        {
            IQueryable<Ward> wardsQuery = _context.Wards.Include(w => w.Beds);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                wardsQuery = wardsQuery.Where(w => w.Name.Contains(searchTerm));
            }

            var wards = await wardsQuery
                .OrderBy(w => w.Name)
                .ToListAsync();

            ViewData["SearchTerm"] = searchTerm;
            return View(wards);
        }

        // GET: Wards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ward = await _context.Wards
                .Include(w => w.Beds)
                .FirstOrDefaultAsync(m => m.WardId == id);

            if (ward == null) return NotFound();

            // Get patients assigned to this ward
            var patientsInWard = await _context.Admissions
                .Include(a => a.Patient)
                .Where(a => a.WardId == id && a.IsActive)
                .Select(a => a.Patient)
                .Distinct()
                .ToListAsync();

            ViewBag.PatientsInWard = patientsInWard;

            // Statistics are now calculated properties in the Ward model
            ViewBag.TotalBeds = ward.TotalBeds;
            ViewBag.OccupiedBeds = ward.OccupiedBeds;
            ViewBag.AvailableBeds = ward.AvailableBeds;
            ViewBag.OccupancyRate = ward.OccupancyRate;

            return View(ward);
        }

        // GET: Wards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Wards/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ward ward)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(ward);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Ward created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Could not create ward, please try again...");
                }
            }

            return View(ward);
        }

        // GET: Wards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ward = await _context.Wards.FindAsync(id);
            if (ward == null) return NotFound();

            return View(ward);
        }

        // POST: Wards/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ward ward)
        {
            if (id != ward.WardId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ward);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Wards.Any(w => w.WardId == id))
                        return NotFound();
                    else
                        throw;
                }
            }

            return View(ward);
        }

        // GET: Wards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var ward = await _context.Wards.FirstOrDefaultAsync(m => m.WardId == id);
            if (ward == null) return NotFound();

            return View(ward);
        }

        // POST: Wards/Delete/5 (Soft delete)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ward = await _context.Wards.FindAsync(id);
            if (ward == null) return NotFound();

            ward.IsActive = false;
            _context.Update(ward);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool WardExists(int id)
        {
            return _context.Wards.Any(e => e.WardId == id);
        }
    }
}