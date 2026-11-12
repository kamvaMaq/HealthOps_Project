using HealthOps_Project.Data;
using HealthOps_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HealthOps_Project.Controllers
{
    public class VisitationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VisitationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Visitation
        public async Task<IActionResult> Index()
        {
            var visits = await _context.Visitations
                .Include(v => v.Patient)
                .Where(v => v.isActive == true) // only include visits for active patients
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync();

            return View(visits);
        }
        // GET: Visitation/Create
        public IActionResult Create()
        {
            // Only include active patients in the dropdown
            ViewBag.PatientList = new SelectList(
                _context.Patients
                    .Where(p => p.IsActive) // <-- only active patients
                    .Select(p => new { p.PatientId, FullName = p.FirstName + " " + p.LastName })
                    .ToList(),
                "PatientId",
                "FullName"
            );

            return View();
        }

        // POST: Visitation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Visitation visitation)
        {
            if (ModelState.IsValid)
            {
                visitation.isActive = true; // ensure new visitation is active
                _context.Add(visitation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Repopulate dropdown if model validation fails
            ViewBag.PatientList = new SelectList(
                _context.Patients
                    .Where(p => p.IsActive) // <-- only active patients
                    .Select(p => new { p.PatientId, FullName = p.FirstName + " " + p.LastName })
                    .ToList(),
                "PatientId",
                "FullName",
                visitation.PatientId
            );

            return View(visitation);
        }


        // GET: Visitation/Edit/5
        // GET: Visitations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitation = await _context.Visitations
                .Include(v => v.Patient)
                .FirstOrDefaultAsync(v => v.VisitId == id);

            if (visitation == null)
            {
                return NotFound();
            }

            // 🩺 Combine PatientId, SouthAfricanID, FirstName, LastName in the dropdown text
            ViewBag.PatientList = new SelectList(
                _context.Patients.Select(p => new
                {
                    p.PatientId,
                    DisplayName = p.PatientId + " - " + p.SouthAfricanID + " - " + p.FirstName + " " + p.LastName
                }),
                "PatientId",
                "DisplayName",
                visitation.PatientId
            );

            return View(visitation);
        }

        // POST: Visitations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Visitation visitation)
        {
            if (id != visitation.VisitId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(visitation);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Visitation updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisitationExists(visitation.VisitId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Repopulate dropdown on validation error
            ViewBag.PatientList = new SelectList(
                _context.Patients.Select(p => new
                {
                    p.PatientId,
                    DisplayName = p.PatientId + " - " + p.SouthAfricanID + " - " + p.FirstName + " " + p.LastName
                }),
                "PatientId",
                "DisplayName",
                visitation.PatientId
            );

            return View(visitation);
        }


        // GET: Visitations/Delete/5

        // GET: Visitations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visit = await _context.Visitations
                .Include(n => n.Patient)
                .FirstOrDefaultAsync(m => m.VisitId == id);

            if (visit == null)
            {
                return NotFound();
            }

            return View(visit);
        }

        // POST: Visitations/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var visit = await _context.Visitations.FindAsync(id);

            if (visit == null)
            {
                return NotFound();
            }

            try
            {
                visit.isActive = false; // soft delete
                visit.Status = "Cancelled";
                _context.Update(visit);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                // Log the exception
                ModelState.AddModelError("", "Unable to delete visit. Please try again.");
                return View(await _context.Visitations.Include(n => n.Patient).FirstOrDefaultAsync(m => m.VisitId == id));
            }
        }

        // Helper
        private bool VisitationExists(int id)
        {
            return _context.Visitations.Any(e => e.VisitId == id);
        }



    }
}
