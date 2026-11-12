using HealthOps_Project.Data;
using HealthOps_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HealthOps_Project.Controllers
{
    [Authorize(Roles = "Admin,Doctor,Nurse")]
    public class AllergiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AllergiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Allergies
        public async Task<IActionResult> Index(string searchString, AllergenType? allergenType, AllergySeverity? severity)
        {
            var allergies = _context.Allergies.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                allergies = allergies.Where(a => a.AllergyName.Contains(searchString));
            }

            if (allergenType.HasValue)
            {
                allergies = allergies.Where(a => a.AllergenType == allergenType);
            }

            if (severity.HasValue)
            {
                allergies = allergies.Where(a => a.SeverityLevel == severity);
            }

            ViewBag.SearchString = searchString;
            ViewBag.AllergenType = allergenType;
            ViewBag.Severity = severity;

            var results = await allergies.OrderBy(a => a.AllergyName).ToListAsync();
            return View(results);
        }

        // GET: Allergies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var allergy = await _context.Allergies.FirstOrDefaultAsync(a => a.AllergyId == id);

            if (allergy == null)
                return NotFound();

            return View(allergy);
        }

        // GET: Allergies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Allergies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Allergy allergy)
        {
            // Remove validation for properties we set in code
            ModelState.Remove("CreatedDate");
            ModelState.Remove("ModifiedDate");
            ModelState.Remove("PatientAllergies");

            if (ModelState.IsValid)
            {
                allergy.CreatedDate = DateTime.Now;
                allergy.IsActive = true;

                _context.Add(allergy);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Allergy created successfully.";
                return RedirectToAction(nameof(Index));
            }

            // Log validation errors for debugging
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine($"Validation Error: {error.ErrorMessage}");
            }

            return View(allergy);
        }

        // GET: Allergies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var allergy = await _context.Allergies.FindAsync(id);
            if (allergy == null)
                return NotFound();

            return View(allergy);
        }

        // POST: Allergies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Allergy allergy)
        {
            if (id != allergy.AllergyId)
                return BadRequest();

            // Remove validation for properties we don't want validated
            ModelState.Remove("ModifiedDate");
            ModelState.Remove("PatientAllergies");

            if (ModelState.IsValid)
            {
                try
                {
                    allergy.ModifiedDate = DateTime.Now;
                    _context.Update(allergy);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Allergy updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AllergyExists(allergy.AllergyId))
                        return NotFound();
                    else
                        throw;
                }
            }

            return View(allergy);
        }

        // GET: Allergies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var allergy = await _context.Allergies
                .FirstOrDefaultAsync(a => a.AllergyId == id);

            if (allergy == null)
                return NotFound();

            return View(allergy);
        }

        // POST: Allergies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var allergy = await _context.Allergies.FindAsync(id);
            if (allergy == null)
                return NotFound();

            // Check if allergy is assigned to any patients
            var patientAllergies = await _context.PatientAllergies
                .CountAsync(pa => pa.AllergyId == id);

            if (patientAllergies > 0)
            {
                TempData["ErrorMessage"] = "Cannot delete allergy. It is assigned to one or more patients.";
                return RedirectToAction(nameof(Index));
            }

            _context.Allergies.Remove(allergy);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Allergy deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool AllergyExists(int id)
        {
            return _context.Allergies.Any(e => e.AllergyId == id);
        }
    }
}
