using HealthOps_Project.Data;
using HealthOps_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HealthOps_Project.Controllers
{
    [Authorize(Roles = "Nurse,NursingSister,Doctor,Admin")]
    public class MedicationAdministrationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicationAdministrationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================
        // SCHEDULED MEDICATIONS (Schedule 5-7)
        // ==========================

        public async Task<IActionResult> Scheduled()
        {
            var scheduledMeds = await _context.Set<ScheduledMedication>()
                .Include(s => s.Medication)
                .Include(s => s.Patient)
                .Where(s => s.ScheduledMedicationStatus != ScheduledMedicationStatus.Deleted)
                .ToListAsync();

            return View(scheduledMeds);
        }

        public async Task<IActionResult> ScheduledDetails(int? id)
        {
            if (id == null) return NotFound();

            var medication = await _context.Set<ScheduledMedication>()
                .Include(m => m.Medication)
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medication == null) return NotFound();

            return View(medication);
        }

        public IActionResult CreateScheduled()
        {
            PopulateScheduledDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateScheduled(ScheduledMedication model)
        {
            // Validate medication schedule
            var medication = await _context.Medications.FindAsync(model.MedicationId);
            if (medication != null && !medication.IsScheduledMedication())
            {
                ModelState.AddModelError("MedicationId",
                    "Only Schedule 5, 6, or 7 medications can be administered as scheduled medications.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Scheduled medication added successfully.";
                return RedirectToAction(nameof(Scheduled));
            }

            PopulateScheduledDropdowns();
            return View(model);
        }

        public async Task<IActionResult> EditScheduled(int? id)
        {
            if (id == null) return NotFound();

            var medication = await _context.Set<ScheduledMedication>().FindAsync(id);
            if (medication == null) return NotFound();

            PopulateScheduledDropdowns();
            return View(medication);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditScheduled(int id, ScheduledMedication model)
        {
            if (id != model.Id) return NotFound();

            // Validate medication schedule
            var medication = await _context.Medications.FindAsync(model.MedicationId);
            if (medication != null && !medication.IsScheduledMedication())
            {
                ModelState.AddModelError("MedicationId",
                    "Only Schedule 5, 6, or 7 medications can be administered as scheduled medications.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Scheduled medication updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduledMedicationExists(model.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Scheduled));
            }

            PopulateScheduledDropdowns();
            return View(model);
        }

        public async Task<IActionResult> DeleteScheduled(int? id)
        {
            if (id == null) return NotFound();

            var med = await _context.Set<ScheduledMedication>()
                .Include(m => m.Medication)
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (med == null) return NotFound();

            return View(med);
        }

        [HttpPost, ActionName("DeleteScheduled")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteScheduledConfirmed(int id)
        {
            var med = await _context.Set<ScheduledMedication>().FindAsync(id);
            if (med != null)
            {
                med.ScheduledMedicationStatus = ScheduledMedicationStatus.Deleted;
                _context.Update(med);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Scheduled medication deleted successfully.";
            }

            return RedirectToAction(nameof(Scheduled));
        }

        // ==========================
        // NON-SCHEDULED MEDICATIONS (Schedule 1-4)
        // ==========================

        public async Task<IActionResult> NonScheduled()
        {
            var nonScheduledMeds = await _context.Set<NonScheduledMedication>()
                .Include(n => n.Medication)
                .Include(n => n.Patient)
                .Where(n => n.Status != NonScheduledMedicationStatus.Deleted)
                .ToListAsync();

            return View(nonScheduledMeds);
        }

        public IActionResult CreateNonScheduled()
        {
            PopulateNonScheduledDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNonScheduled(NonScheduledMedication model)
        {
            // Validate medication schedule
            var medication = await _context.Medications.FindAsync(model.MedicationId);
            if (medication != null && !medication.IsNonScheduledMedication())
            {
                ModelState.AddModelError("MedicationId",
                    "Only Schedule 1, 2, 3, or 4 medications can be administered as non-scheduled medications.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Non-scheduled medication added successfully.";
                return RedirectToAction(nameof(NonScheduled));
            }

            PopulateNonScheduledDropdowns();
            return View(model);
        }

        public async Task<IActionResult> DeleteNonScheduled(int? id)
        {
            if (id == null) return NotFound();

            var med = await _context.Set<NonScheduledMedication>()
                .Include(m => m.Medication)
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (med == null) return NotFound();

            return View(med);
        }

        [HttpPost, ActionName("DeleteNonScheduled")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNonScheduledConfirmed(int id)
        {
            var med = await _context.Set<NonScheduledMedication>().FindAsync(id);
            if (med != null)
            {
                med.Status = NonScheduledMedicationStatus.Deleted;
                _context.Update(med);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Non-scheduled medication deleted successfully.";
            }

            return RedirectToAction(nameof(NonScheduled));
        }

        // ==========================
        // MEDICATION CRUD
        // ==========================

        public async Task<IActionResult> Medications()
        {
            var meds = await _context.Medications
                .Where(m => m.DeletionStatus == MedicationStatus.Active)
                .ToListAsync();
            return View(meds);
        }

        public async Task<IActionResult> DetailsMedication(int? id)
        {
            if (id == null) return NotFound();

            var medication = await _context.Medications
                .FirstOrDefaultAsync(m => m.MedicationId == id);

            if (medication == null) return NotFound();

            return View(medication);
        }

        public IActionResult CreateMedication()
        {
            PopulateMedicationDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMedication(Medication model)
        {
            if (ModelState.IsValid)
            {
                model.DeletionStatus = MedicationStatus.Active;
                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Medication created successfully.";
                return RedirectToAction(nameof(Medications));
            }

            PopulateMedicationDropdowns(model);
            return View(model);
        }

        public async Task<IActionResult> EditMedication(int? id)
        {
            if (id == null) return NotFound();

            var medication = await _context.Medications.FindAsync(id);
            if (medication == null) return NotFound();

            PopulateMedicationDropdowns(medication);
            return View(medication);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMedication(int id, Medication model)
        {
            if (id != model.MedicationId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Medication updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Medications.Any(m => m.MedicationId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Medications));
            }

            PopulateMedicationDropdowns(model);
            return View(model);
        }

        public async Task<IActionResult> DeleteMedication(int? id)
        {
            if (id == null) return NotFound();

            var medication = await _context.Medications.FirstOrDefaultAsync(m => m.MedicationId == id);
            if (medication == null) return NotFound();

            return View(medication);
        }

        [HttpPost, ActionName("DeleteMedication")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMedicationConfirmed(int id)
        {
            var medication = await _context.Medications.FindAsync(id);
            if (medication != null)
            {
                medication.DeletionStatus = MedicationStatus.Deleted;
                _context.Update(medication);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Medication deleted successfully.";
            }
            return RedirectToAction(nameof(Medications));
        }

        // ==========================
        // HELPER METHODS
        // ==========================

        private void PopulateScheduledDropdowns()
        {
            // Only show Schedule 5-7 medications
            ViewData["Medications"] = _context.Set<Medication>()
                .Where(m => m.DeletionStatus == MedicationStatus.Active &&
                           m.Schedule >= MedicationSchedule.Schedule5)
                .ToList();

            // Get all patients - adjust the filter based on your Patient model
            ViewData["Patients"] = _context.Set<Patient>().ToList();
        }

        private void PopulateNonScheduledDropdowns()
        {
            // Only show Schedule 1-4 medications
            ViewData["Medications"] = _context.Set<Medication>()
                .Where(m => m.DeletionStatus == MedicationStatus.Active &&
                           m.Schedule <= MedicationSchedule.Schedule4)
                .ToList();

            // Get all patients - adjust the filter based on your Patient model
            ViewData["Patients"] = _context.Set<Patient>().ToList();
        }

        private void PopulateMedicationDropdowns(Medication? medication = null)
        {
            ViewBag.TypeList = Enum.GetValues(typeof(MedicationType))
                .Cast<MedicationType>()
                .Select(t => new SelectListItem
                {
                    Value = t.ToString(),
                    Text = t.ToString(),
                    Selected = medication != null && medication.Type == t
                })
                .ToList();

            ViewBag.ScheduleList = Enum.GetValues(typeof(MedicationSchedule))
                .Cast<MedicationSchedule>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString(),
                    Selected = medication != null && medication.Schedule == s
                })
                .ToList();

            ViewBag.StatusList = Enum.GetValues(typeof(MedicationStatus))
                .Cast<MedicationStatus>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString(),
                    Selected = medication != null && medication.DeletionStatus == s
                })
                .ToList();
        }

        private bool ScheduledMedicationExists(int id)
        {
            return _context.Set<ScheduledMedication>().Any(e => e.Id == id);
        }
    }
}
