using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using HealthOps_Project.Data;
using HealthOps_Project.Models;
using HealthOps_Project.Services;

namespace HealthOps_Project.Controllers
{
    [Authorize(Roles = "Doctor,ScriptManager, NursingSister")]
    public class PrescriptionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;
        private readonly IPdfService _pdfService;

        public PrescriptionsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService,
            IPdfService pdfService)
        {
            _context = context;
            _userManager = userManager;
            _notificationService = notificationService;
            _pdfService = pdfService;
        }

        // GET: Prescriptions
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var prescriptions = _context.Prescriptions
                .Include(p => p.Patient)
                .OrderByDescending(p => p.CreatedAt)
                .AsQueryable();

            // If user is a doctor, show only their prescriptions
            if (currentUser?.Role == UserRole.Doctor)
            {
                prescriptions = prescriptions.Where(p => p.PrescribingDoctor == currentUser.FullName);
            }

            return View(await prescriptions.ToListAsync());
        }

        // GET: Prescriptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            Prescription prescription;

            if (id == null)
            {
                // Get the first or latest prescription in the database
                prescription = await _context.Prescriptions
                    .Include(p => p.Patient)
                    .Include(p => p.AdministrationRecords)
                    .OrderByDescending(p => p.CreatedAt)
                    .FirstOrDefaultAsync();

                if (prescription == null)
                {
                    return NotFound();
                }
            }
            else
            {
                // Existing logic if ID is provided
                prescription = await _context.Prescriptions
                    .Include(p => p.Patient)
                    .Include(p => p.AdministrationRecords)
                    .FirstOrDefaultAsync(m => m.PrescriptionId == id);

                if (prescription == null)
                {
                    return NotFound();
                }
            }

            // Authorization check for doctors
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Role == UserRole.Doctor && prescription.PrescribingDoctor != currentUser.FullName)
            {
                return Forbid();
            }

            return View(prescription);
        }

        // GET: Prescriptions/Create
        public IActionResult Create()
        {
            ViewBag.PatientList = GetPatientsSelectList();
            return View();
        }

        // POST: Prescriptions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Prescription prescription)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Debug: Check what values are coming in
            Console.WriteLine($"PatientId received: {prescription.PatientId}");
            Console.WriteLine($"MedicationName: {prescription.MedicationName}");

            // Auto-assign prescribing doctor from logged-in user
            if (currentUser != null)
            {
                prescription.PrescribingDoctor = currentUser.FullName;
            }

            prescription.CreatedAt = DateTime.Now;

            // Initialize collections to avoid validation errors
            prescription.MedicationDeliveries = new List<MedicationDelivery>();
            prescription.AdministrationRecords = new List<MedicationAdministration>();

            // Set default values for optional fields if they are null
            if (string.IsNullOrEmpty(prescription.Instructions))
            {
                prescription.Instructions = "Take as directed";
            }

            if (string.IsNullOrEmpty(prescription.Notes))
            {
                prescription.Notes = "No additional notes";
            }

            // Clear ModelState and re-validate
            ModelState.Clear();
            TryValidateModel(prescription);

            // Debug: Check ModelState after re-validation
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
                ViewBag.ModelErrors = errors.Select(e => e.ErrorMessage).ToList();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(prescription);
                    await _context.SaveChangesAsync();

                    // Send notification for new prescription
                    await _notificationService.NotifyNewPrescriptionAsync(prescription);

                    TempData["Success"] = "Prescription created successfully and notifications sent!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating prescription: " + ex.Message);
                    Console.WriteLine($"Database Error: {ex.Message}");
                }
            }

            ViewBag.PatientList = GetPatientsSelectList();
            return View(prescription);
        }

        // GET: Prescriptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
            {
                return NotFound();
            }

            // Authorization check
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Role == UserRole.Doctor && prescription.PrescribingDoctor != currentUser.FullName)
            {
                return Forbid();
            }

            ViewBag.PatientList = GetPatientsSelectList();
            return View(prescription);
        }

        // POST: Prescriptions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Prescription prescription)
        {
            if (id != prescription.PrescriptionId)
            {
                return NotFound();
            }

            // Authorization check
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Role == UserRole.Doctor)
            {
                var existingPrescription = await _context.Prescriptions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.PrescriptionId == id);

                if (existingPrescription?.PrescribingDoctor != currentUser.FullName)
                {
                    return Forbid();
                }

                // Ensure prescribing doctor remains the same
                prescription.PrescribingDoctor = currentUser.FullName;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    prescription.UpdatedAt = DateTime.UtcNow;
                    _context.Update(prescription);
                    await _context.SaveChangesAsync();

                    // Send notification for prescription update
                    await _notificationService.NotifyPrescriptionUpdatedAsync(prescription);

                    TempData["Success"] = "Prescription updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrescriptionExists(prescription.PrescriptionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.PatientList = GetPatientsSelectList();
            return View(prescription);
        }

        // GET: Prescriptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.PrescriptionId == id);

            if (prescription == null)
            {
                return NotFound();
            }

            // Authorization check
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Role == UserRole.Doctor && prescription.PrescribingDoctor != currentUser.FullName)
            {
                return Forbid();
            }

            return View(prescription);
        }

        // POST: Prescriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);

            if (prescription == null)
            {
                return NotFound();
            }

            // Authorization check
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Role == UserRole.Doctor && prescription.PrescribingDoctor != currentUser.FullName)
            {
                return Forbid();
            }

            _context.Prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();

            // Send notification for prescription deletion
            await _notificationService.NotifyPrescriptionDeletedAsync(prescription);

            TempData["Success"] = "Prescription deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // PDF Generation Methods
        public async Task<IActionResult> GeneratePdf(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Prescription ID not provided.";
                return RedirectToAction(nameof(Index));
            }

            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(m => m.PrescriptionId == id);

            if (prescription == null)
            {
                TempData["Error"] = "Prescription not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var pdfBytes = _pdfService.GeneratePrescriptionPdf(prescription);
                var fileName = $"Prescription-{prescription.PrescriptionId}-{DateTime.Now:yyyyMMdd}.pdf";

                TempData["Success"] = "Prescription PDF generated successfully!";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to generate PDF: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        public async Task<IActionResult> GenerateAllPdf()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var prescriptions = _context.Prescriptions
                .Include(p => p.Patient)
                .OrderByDescending(p => p.CreatedAt)
                .AsQueryable();

            // If user is a doctor, show only their prescriptions
            if (currentUser?.Role == UserRole.Doctor)
            {
                prescriptions = prescriptions.Where(p => p.PrescribingDoctor == currentUser.FullName);
            }

            var prescriptionList = await prescriptions.ToListAsync();

            if (!prescriptionList.Any())
            {
                TempData["Info"] = "No prescriptions found to generate PDF.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var pdfBytes = _pdfService.GeneratePrescriptionsReport(prescriptionList);
                var fileName = $"AllPrescriptions-{DateTime.Now:yyyyMMdd_HHmm}.pdf";

                TempData["Success"] = "All prescriptions PDF generated successfully!";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to generate PDF report: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Script Processing Methods
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ScriptManager,Admin")]
        public async Task<IActionResult> ProcessScript(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
            {
                TempData["Error"] = "Prescription not found.";
                return RedirectToAction(nameof(Index));
            }

            prescription.Status = "Processed";
            prescription.UpdatedAt = DateTime.UtcNow;

            _context.Update(prescription);
            await _context.SaveChangesAsync();

            // Send notification for script processing
            await _notificationService.NotifyScriptProcessedAsync(prescription);

            TempData["Success"] = "Script processed and forwarded to pharmacy!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ScriptManager,Admin")]
        public async Task<IActionResult> MarkAsDispensed(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
            {
                TempData["Error"] = "Prescription not found.";
                return RedirectToAction(nameof(Index));
            }

            prescription.Status = "Dispensed";
            prescription.UpdatedAt = DateTime.UtcNow;

            _context.Update(prescription);
            await _context.SaveChangesAsync();

            // Send notification for medication dispensed
            await _notificationService.NotifyMedicationDispensedAsync(prescription);

            TempData["Success"] = "Medication marked as dispensed!";
            return RedirectToAction(nameof(Index));
        }

        // Helper Methods
        private bool PrescriptionExists(int id)
        {
            return _context.Prescriptions.Any(e => e.PrescriptionId == id);
        }

        private List<SelectListItem> GetPatientsSelectList()
        {
            return _context.Patients
                .Select(p => new SelectListItem
                {
                    Value = p.PatientId.ToString(),
                    Text = $"{p.SouthAfricanID} - {p.FirstName} {p.LastName}"
                })
                .ToList();
        }
    }
}