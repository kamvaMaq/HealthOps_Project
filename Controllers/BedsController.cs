using HealthOps_Project.Data;
using HealthOps_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthOps_Project.Controllers
{
    public class BedsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BedsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Beds
        public async Task<IActionResult> Index()
        {
            var beds = await _context.Beds
                .Include(b => b.Ward)
                .Include(b => b.Room)
                .ToListAsync();
            return View(beds);
        }

        // GET: Beds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var bed = await _context.Beds
                .Include(b => b.Ward)
                .Include(b => b.Room)
                .Include(b => b.Patient)
                .FirstOrDefaultAsync(m => m.BedId == id);

            if (bed == null) return NotFound();

            return View(bed);
        }

        // GET: Beds/Create
        public IActionResult Create()
        {
            ViewData["WardId"] = new SelectList(_context.Wards, "WardId", "Name");
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber");
            return View();
        }

        // POST: Beds/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bed bed)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(bed);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "✅ Bed added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Could not create bed. Please try again.");
                }
            }

            ViewData["WardId"] = new SelectList(_context.Wards, "WardId", "Name", bed.WardId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber", bed.RoomId);
            return View(bed);
        }

        // GET: Beds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var bed = await _context.Beds.FindAsync(id);
            if (bed == null) return NotFound();

            ViewData["WardId"] = new SelectList(_context.Wards, "WardId", "Name", bed.WardId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber", bed.RoomId);
            return View(bed);
        }

        // POST: Beds/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Bed bed)
        {
            if (id != bed.BedId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bed);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "✅ Bed updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Beds.Any(e => e.BedId == id))
                        return NotFound();
                    else
                        throw;
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Could not update bed. Please try again.");
                }
            }

            ViewData["WardId"] = new SelectList(_context.Wards, "WardId", "Name", bed.WardId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber", bed.RoomId);
            return View(bed);
        }

        // GET: Beds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var bed = await _context.Beds
                .Include(b => b.Ward)
                .FirstOrDefaultAsync(m => m.BedId == id);

            if (bed == null) return NotFound();

            return View(bed);
        }

        // POST: Beds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bed = await _context.Beds.FindAsync(id);
            if (bed == null) return NotFound();

            _context.Beds.Remove(bed);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "🗑 Bed deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool BedExists(int id)
        {
            return _context.Beds.Any(e => e.BedId == id);
        }
    }
}