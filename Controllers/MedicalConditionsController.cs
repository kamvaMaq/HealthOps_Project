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
    public class MedicalConditionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicalConditionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MedicalConditions
        public async Task<IActionResult> Index(string searchString, ConditionCategory? category, string icd10Code)
        {
            var conditions = _context.MedicalConditions.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                conditions = conditions.Where(c =>
                    c.ConditionName.Contains(searchString) ||
                    c.Description.Contains(searchString));
            }

            if (category.HasValue)
            {
                conditions = conditions.Where(c => c.Category == category);
            }

            if (!string.IsNullOrEmpty(icd10Code))
            {
                conditions = conditions.Where(c => c.ICD10Code.Contains(icd10Code));
            }

            ViewBag.SearchString = searchString;
            ViewBag.Category = category;
            ViewBag.ICD10Code = icd10Code;

            var result = await conditions.OrderBy(c => c.ConditionName).ToListAsync();
            return View(result);
        }

        // GET: MedicalConditions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var condition = await _context.MedicalConditions.FirstOrDefaultAsync(c => c.ConditionId == id);
            if (condition == null)
                return NotFound();

            return View(condition);
        }

        // GET: MedicalConditions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MedicalConditions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicalCondition condition)
        {
            // Remove validation for properties we set in code
            ModelState.Remove("CreatedDate");
            ModelState.Remove("ModifiedDate");
            ModelState.Remove("PatientConditions");

            if (ModelState.IsValid)
            {
                condition.CreatedDate = DateTime.Now;
                condition.IsActive = true;
                _context.Add(condition);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Medical condition created successfully.";
                return RedirectToAction(nameof(Index));
            }

            // Debug: Log validation errors
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    System.Diagnostics.Debug.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
            }

            return View(condition);
        }

        // GET: MedicalConditions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var condition = await _context.MedicalConditions.FindAsync(id);
            if (condition == null)
                return NotFound();

            return View(condition);
        }

        // POST: MedicalConditions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MedicalCondition condition)
        {
            if (id != condition.ConditionId)
                return BadRequest();

            // Remove validation for properties we don't want validated
            ModelState.Remove("ModifiedDate");
            ModelState.Remove("PatientConditions");

            if (ModelState.IsValid)
            {
                try
                {
                    condition.ModifiedDate = DateTime.Now;
                    _context.Update(condition);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Medical condition updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConditionExists(condition.ConditionId))
                        return NotFound();
                    else
                        throw;
                }
            }

            return View(condition);
        }

        // GET: MedicalConditions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var condition = await _context.MedicalConditions.FirstOrDefaultAsync(c => c.ConditionId == id);
            if (condition == null)
                return NotFound();

            return View(condition);
        }

        // POST: MedicalConditions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var condition = await _context.MedicalConditions.FindAsync(id);
            if (condition == null)
                return NotFound();

            // Check if condition is assigned to any patients
            var patientConditions = await _context.PatientConditions
                .CountAsync(pc => pc.ConditionId == id);

            if (patientConditions > 0)
            {
                TempData["ErrorMessage"] = "Cannot delete condition. It is assigned to one or more patients.";
                return RedirectToAction(nameof(Index));
            }

            _context.MedicalConditions.Remove(condition);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Medical condition deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool ConditionExists(int id)
        {
            return _context.MedicalConditions.Any(e => e.ConditionId == id);
        }
    }
}
