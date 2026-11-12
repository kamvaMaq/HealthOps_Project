using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthOps_Project.Data;
using HealthOps_Project.Models;

namespace HealthOps_Project.Controllers
{
    public class VitalSignsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VitalSignsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /VitalSigns?patientId=5 (optional)
        public async Task<IActionResult> Index(int? patientId)
        {
            IQueryable<VitalSign> vitalsQuery = _context.VitalSigns.Include(v => v.Patient);

            if (patientId.HasValue)
            {
                var patient = await _context.Patients.FindAsync(patientId.Value);
                if (patient == null)
                    return NotFound();

                vitalsQuery = vitalsQuery.Where(v => v.PatientId == patientId.Value);
                ViewBag.Patient = patient;
            }
            else
            {
                ViewBag.Patient = null;
            }

            var vitals = await vitalsQuery
                .OrderByDescending(v => v.RecordedDate)
                .ToListAsync();

            return View("Index", vitals);
        }

        // GET: VitalSigns/Create?patientId=5
        public async Task<IActionResult> Create(int? patientId)
        {
            if (!patientId.HasValue)
                return RedirectToAction(nameof(SelectPatient));

            var patient = await _context.Patients.FindAsync(patientId.Value);
            if (patient == null)
                return NotFound();

            ViewBag.Patient = patient;

            var vitals = new VitalSign
            {
                PatientId = patientId.Value,
                RecordedBy = User.Identity?.Name ?? "Unknown" // Set default value
            };

            return View(vitals);
        }

        // POST: VitalSigns/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VitalSign vitalSign)
        {
            // Check if PatientId is valid
            if (vitalSign.PatientId == 0)
            {
                ModelState.AddModelError("", "Patient ID is required.");
                ViewBag.Patient = await _context.Patients.FindAsync(vitalSign.PatientId);
                return View(vitalSign);
            }

            // Verify patient exists
            var patient = await _context.Patients.FindAsync(vitalSign.PatientId);
            if (patient == null)
            {
                ModelState.AddModelError("", "Patient not found.");
                return View(vitalSign);
            }

            // Set RecordedBy and RecordedDate
            vitalSign.RecordedDate = DateTime.Now;
            vitalSign.RecordedBy = User.Identity?.Name ?? "Unknown User";

            // Remove RecordedBy from ModelState validation since we're setting it manually
            ModelState.Remove("RecordedBy");
            ModelState.Remove("RecordedDate");
            ModelState.Remove("Patient"); // Remove navigation property from validation

            // Check ModelState validity
            if (!ModelState.IsValid)
            {
                // Log validation errors for debugging
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }

                ViewBag.Patient = patient;
                return View(vitalSign);
            }

            try
            {
                _context.VitalSigns.Add(vitalSign);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Vitals recorded successfully!";
                return RedirectToAction(nameof(Index), new { patientId = vitalSign.PatientId });
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error saving vital signs: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while saving the vital signs.");
                ViewBag.Patient = patient;
                return View(vitalSign);
            }
        }

        // GET: VitalSigns/SelectPatient
        public async Task<IActionResult> SelectPatient()
        {
            var patients = await _context.Patients
                .Where(p => p.AdmissionDate != null && p.IsActive)
                .OrderBy(p => p.LastName)
                .ToListAsync();

            return View(patients);
        }

        // GET: VitalSigns/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var vitalSign = await _context.VitalSigns
                .Include(v => v.Patient)
                .FirstOrDefaultAsync(v => v.VitalId == id);

            if (vitalSign == null)
                return NotFound();

            return View(vitalSign);
        }

        // GET: VitalSigns/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var vitalSign = await _context.VitalSigns
                .Include(v => v.Patient)
                .FirstOrDefaultAsync(v => v.VitalId == id);

            if (vitalSign == null)
                return NotFound();

            ViewBag.Patient = vitalSign.Patient;
            return View(vitalSign);
        }

        // POST: VitalSigns/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VitalSign vitalSign)
        {
            if (id != vitalSign.VitalId)
                return NotFound();

            // Get patient for ViewBag in case validation fails
            var patient = await _context.Patients.FindAsync(vitalSign.PatientId);
            if (patient == null)
                return NotFound();

            // Remove navigation properties from validation
            ModelState.Remove("Patient");
            ModelState.Remove("RecordedBy");

            if (!ModelState.IsValid)
            {
                ViewBag.Patient = patient;
                return View(vitalSign);
            }

            try
            {
                var existingVital = await _context.VitalSigns.FindAsync(id);
                if (existingVital == null)
                    return NotFound();

                // Update properties
                existingVital.Temperature = vitalSign.Temperature;
                existingVital.BloodPressure = vitalSign.BloodPressure;
                existingVital.HeartRate = vitalSign.HeartRate;
                existingVital.OxygenSaturation = vitalSign.OxygenSaturation;
                existingVital.Weight = vitalSign.Weight;
                existingVital.BloodSugar = vitalSign.BloodSugar;
                existingVital.Notes = vitalSign.Notes;
                // Keep original RecordedBy and RecordedDate

                _context.Update(existingVital);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Vitals updated successfully!";
                return RedirectToAction(nameof(Index), new { patientId = vitalSign.PatientId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VitalSignExists(vitalSign.VitalId))
                    return NotFound();
                else
                    throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating vital signs: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while updating the vital signs.");
                ViewBag.Patient = patient;
                return View(vitalSign);
            }
        }

        // GET: VitalSigns/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var vitalSign = await _context.VitalSigns
                .Include(v => v.Patient)
                .FirstOrDefaultAsync(v => v.VitalId == id);

            if (vitalSign == null)
                return NotFound();

            return View(vitalSign);
        }

        // POST: VitalSigns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var vitalSign = await _context.VitalSigns.FindAsync(id);
                if (vitalSign == null)
                    return NotFound();

                int patientId = vitalSign.PatientId;

                _context.VitalSigns.Remove(vitalSign);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Vitals deleted successfully!";
                return RedirectToAction(nameof(Index), new { patientId = patientId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting vital signs: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while deleting the vital signs.";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool VitalSignExists(int id)
        {
            return _context.VitalSigns.Any(e => e.VitalId == id);
        }
    }
}
