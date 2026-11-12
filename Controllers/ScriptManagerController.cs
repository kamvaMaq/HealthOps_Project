using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using HealthOps_Project.Models;
using HealthOps_Project.Services;
using HealthOps_Project.Data;

namespace HealthOps_Project.Controllers
{
    [Authorize(Roles = "ScriptManager")]
    public class ScriptManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPdfService _pdfService;
        private readonly INotificationService _notificationService;

        public ScriptManagerController(ApplicationDbContext context, IPdfService pdfService, INotificationService notificationService)
        {
            _context = context;
            _pdfService = pdfService;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            var scripts = await _context.Prescriptions
                .Include(p => p.Patient)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(scripts);
        }

        public async Task<IActionResult> Details(int id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(p => p.PrescriptionId == id);
            if (prescription == null)
            {
                TempData["Error"] = "Prescription not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(prescription);
        }

        public async Task<IActionResult> PendingScripts()
        {
            var pendingScripts = await _context.Prescriptions
                .Include(p => p.Patient)
                .Where(p => p.Status == "New" || p.Status == "Pending")
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View("Index", pendingScripts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsProcessed(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
            {
                TempData["Error"] = "Prescription not found.";
                return RedirectToAction(nameof(Index));
            }

            prescription.Status = "Processed";
            prescription.UpdatedAt = System.DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Send notification
            await _notificationService.NotifyScriptProcessedAsync(prescription);

            TempData["Success"] = "Script marked as processed and forwarded to pharmacy!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsDispensed(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
            {
                TempData["Error"] = "Prescription not found.";
                return RedirectToAction(nameof(Index));
            }

            prescription.Status = "Dispensed";
            prescription.UpdatedAt = System.DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Send notification
            await _notificationService.NotifyMedicationDispensedAsync(prescription);

            TempData["Success"] = "Medication marked as dispensed!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GeneratePdf(int id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(p => p.PrescriptionId == id);

            if (prescription == null)
            {
                TempData["Error"] = "Prescription not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var pdf = _pdfService.GeneratePrescriptionPdf(prescription);
                TempData["Success"] = "PDF generated successfully!";
                return File(pdf, "application/pdf", $"Prescription_{prescription.PrescriptionId}.pdf");
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = "Failed to generate PDF.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        public async Task<IActionResult> GenerateAllPdf()
        {
            try
            {
                var prescriptions = await _context.Prescriptions
                    .Include(p => p.Patient)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                if (!prescriptions.Any())
                {
                    TempData["Info"] = "No prescriptions found to generate PDF.";
                    return RedirectToAction(nameof(Index));
                }

                var pdf = _pdfService.GeneratePrescriptionsReport(prescriptions);
                TempData["Success"] = "All prescriptions PDF generated successfully!";
                return File(pdf, "application/pdf", $"AllPrescriptions_{System.DateTime.Now:yyyyMMdd_HHmm}.pdf");
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = "Failed to generate PDF report.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> GeneratePendingPdf()
        {
            try
            {
                var pendingScripts = await _context.Prescriptions
                    .Include(p => p.Patient)
                    .Where(p => p.Status == "New" || p.Status == "Pending")
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                if (!pendingScripts.Any())
                {
                    TempData["Info"] = "No pending prescriptions found to generate PDF.";
                    return RedirectToAction(nameof(Index));
                }

                var pdf = _pdfService.GeneratePrescriptionsReport(pendingScripts);
                TempData["Success"] = "Pending prescriptions PDF generated successfully!";
                return File(pdf, "application/pdf", $"PendingPrescriptions_{System.DateTime.Now:yyyyMMdd_HHmm}.pdf");
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = "Failed to generate PDF report.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}