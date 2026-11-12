using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthOps_Project.Data;
using HealthOps_Project.Models;
using System.Linq;

namespace HealthOps_Project.Controllers
{
    public class TreatmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TreatmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Treatments
        public async Task<IActionResult> Index(int? patientId)
        {
            var treatments = _context.Treatments.Include(t => t.Patient).AsQueryable();

            if (patientId.HasValue)
            {
                treatments = treatments.Where(t => t.PatientId == patientId.Value);

                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.PatientId == patientId.Value);

                if (patient != null)
                {
                    ViewData["PatientName"] = $"{patient.FirstName} {patient.LastName}";
                }
            }

            return View(await treatments.ToListAsync());
        }

        // GET: Treatments/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var treatment = await _context.Treatments
                .Include(t => t.Patient)
                .FirstOrDefaultAsync(m => m.TreatmentId == id);

            if (treatment == null) return NotFound();

            return View(treatment);
        }

        // GET: Treatments/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        // POST: Treatments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Treatment treatment)
        {
            // Remove Patient navigation property from validation
            ModelState.Remove("Patient");

            if (ModelState.IsValid)
            {
                _context.Add(treatment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateDropdowns(treatment);
            return View(treatment);
        }

        // GET: Treatments/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var treatment = await _context.Treatments.FindAsync(id);
            if (treatment == null) return NotFound();

            PopulateDropdowns(treatment);
            return View(treatment);
        }

        // POST: Treatments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Treatment treatment)
        {
            if (id != treatment.TreatmentId) return NotFound();

            // Remove Patient navigation property from validation
            ModelState.Remove("Patient");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(treatment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Treatments.Any(e => e.TreatmentId == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            PopulateDropdowns(treatment);
            return View(treatment);
        }

        // GET: Treatments/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var treatment = await _context.Treatments
                .Include(t => t.Patient)
                .FirstOrDefaultAsync(m => m.TreatmentId == id);

            if (treatment == null) return NotFound();

            return View(treatment);
        }

        // POST: Treatments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var treatment = await _context.Treatments.FindAsync(id);
            if (treatment != null)
            {
                _context.Treatments.Remove(treatment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // =============================
        // Helper method to populate dropdowns
        // =============================
        private void PopulateDropdowns(Treatment? treatment = null)
        {
            // Create a more user-friendly patient list showing full name
            var patients = _context.Patients
                .Select(p => new
                {
                    p.PatientId,
                    FullName = p.FirstName + " " + p.LastName
                })
                .ToList();

            ViewBag.PatientId = new SelectList(patients, "PatientId", "FullName", treatment?.PatientId);

            ViewBag.TreatmentType = Enum.GetValues(typeof(TreatmentType))
                                        .Cast<TreatmentType>()
                                        .Select(tt => new SelectListItem
                                        {
                                            Value = ((int)tt).ToString(),
                                            Text = tt.ToString(),
                                            Selected = treatment != null && treatment.TreatmentType == tt
                                        })
                                        .ToList();
        }
    }
}



