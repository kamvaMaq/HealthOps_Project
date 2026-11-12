using HealthOps_Project.Data;
using HealthOps_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HealthOps_Project.Controllers
{
    [Authorize(Roles = "Admin,ScriptManager")]
    public class PrescriptionDeliveriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionDeliveriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Index
        public async Task<IActionResult> Index()
        {
            var deliveries = await _context.PrescriptionDeliveries
                .Include(d => d.Prescription)
                    .ThenInclude(p => p.Patient)
                .ToListAsync();
            return View(deliveries);
        }

        // GET: Delivery Report
        public async Task<IActionResult> DeliveryReport()
        {
            var deliveries = await _context.PrescriptionDeliveries
                .Include(d => d.Prescription)
                    .ThenInclude(p => p.Patient)
                .OrderByDescending(d => d.RequestedAt)
                .ToListAsync();

            return View(deliveries);
        }

        // GET: Generate Report
        public async Task<IActionResult> GenerateReport()
        {
            var deliveries = await _context.PrescriptionDeliveries
                .Include(d => d.Prescription)
                    .ThenInclude(p => p.Patient)
                .OrderByDescending(d => d.RequestedAt)
                .ToListAsync();

            return View("DeliveryReport", deliveries);
        }

        // GET: Download PDF Report
        public async Task<IActionResult> DownloadPdfReport()
        {
            var deliveries = await _context.PrescriptionDeliveries
                .Include(d => d.Prescription)
                    .ThenInclude(p => p.Patient)
                .OrderByDescending(d => d.RequestedAt)
                .ToListAsync();

            // For now, return the view - you can implement PDF generation later
            // using libraries like Rotativa, DinkToPdf, etc.
            return View("DeliveryReport", deliveries);
        }

        // GET: Delivery Statistics
        public async Task<IActionResult> Statistics()
        {
            var deliveries = await _context.PrescriptionDeliveries
                .Include(d => d.Prescription)
                    .ThenInclude(p => p.Patient)
                .ToListAsync();

            var stats = new
            {
                TotalDeliveries = deliveries.Count,
                PendingDeliveries = deliveries.Count(d => d.Status == "Pending"),
                DeliveredCount = deliveries.Count(d => d.Status == "Delivered"),
                UniquePatients = deliveries.Select(d => d.Prescription?.PatientId).Distinct().Count(),
                AverageDeliveryTime = deliveries
                    .Where(d => d.Status == "Delivered" && d.DeliveredAt.HasValue)
                    .Average(d => (d.DeliveredAt.Value - d.RequestedAt).TotalDays)
            };

            return Json(stats);
        }

        // GET: Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return RedirectToAction(nameof(Index));

            var delivery = await _context.PrescriptionDeliveries
                .Include(d => d.Prescription)
                    .ThenInclude(p => p.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (delivery == null) return RedirectToAction(nameof(Index));
            return View(delivery);
        }

        // GET: Create
        public async Task<IActionResult> Create()
        {
            var prescriptions = await _context.Prescriptions
                .Include(p => p.Patient)
                .Where(p => p.Status == "Active" || p.Status == "Pending")
                .Select(p => new { p.PrescriptionId, DisplayText = $"{p.Patient.FirstName} {p.Patient.LastName} - {p.MedicationName}" })
                .ToListAsync();

            ViewData["PrescriptionId"] = new SelectList(prescriptions, "PrescriptionId", "DisplayText");
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PrescriptionId")] PrescriptionDelivery delivery)
        {
            if (ModelState.IsValid)
            {
                delivery.Status = "Pending";
                delivery.RequestedAt = DateTime.UtcNow;

                _context.Add(delivery);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Prescription delivery created successfully!";
                return RedirectToAction(nameof(Index));
            }

            var prescriptions = await _context.Prescriptions
                .Include(p => p.Patient)
                .Where(p => p.Status == "Active" || p.Status == "Pending")
                .Select(p => new { p.PrescriptionId, DisplayText = $"{p.Patient.FirstName} {p.Patient.LastName} - {p.MedicationName}" })
                .ToListAsync();

            ViewData["PrescriptionId"] = new SelectList(prescriptions, "PrescriptionId", "DisplayText", delivery.PrescriptionId);
            return View(delivery);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return RedirectToAction(nameof(Index));

            var delivery = await _context.PrescriptionDeliveries.FindAsync(id);
            if (delivery == null) return RedirectToAction(nameof(Index));

            var prescriptions = await _context.Prescriptions
                .Include(p => p.Patient)
                .Where(p => p.Status == "Active" || p.Status == "Pending" || p.Status == "Delivered")
                .Select(p => new { p.PrescriptionId, DisplayText = $"{p.Patient.FirstName} {p.Patient.LastName} - {p.MedicationName}" })
                .ToListAsync();

            ViewData["PrescriptionId"] = new SelectList(prescriptions, "PrescriptionId", "DisplayText", delivery.PrescriptionId);
            return View(delivery);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PrescriptionId,Status,RequestedAt,DeliveredAt")] PrescriptionDelivery delivery)
        {
            if (id != delivery.Id) return RedirectToAction(nameof(Index));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(delivery);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Prescription delivery updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.PrescriptionDeliveries.Any(e => e.Id == delivery.Id))
                        return RedirectToAction(nameof(Index));
                    else throw;
                }
            }

            var prescriptions = await _context.Prescriptions
                .Include(p => p.Patient)
                .Where(p => p.Status == "Active" || p.Status == "Pending" || p.Status == "Delivered")
                .Select(p => new { p.PrescriptionId, DisplayText = $"{p.Patient.FirstName} {p.Patient.LastName} - {p.MedicationName}" })
                .ToListAsync();

            ViewData["PrescriptionId"] = new SelectList(prescriptions, "PrescriptionId", "DisplayText", delivery.PrescriptionId);
            return View(delivery);
        }

        // POST: MarkDelivered
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkDelivered(int id)
        {
            var delivery = await _context.PrescriptionDeliveries.FindAsync(id);
            if (delivery == null) return RedirectToAction(nameof(Index));

            delivery.Status = "Delivered";
            delivery.DeliveredAt = DateTime.UtcNow;

            var prescription = await _context.Prescriptions.FindAsync(delivery.PrescriptionId);
            if (prescription != null)
            {
                prescription.Status = "Delivered";
                prescription.UpdatedAt = DateTime.UtcNow;
                _context.Update(prescription);
            }

            _context.Update(delivery);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Prescription marked as delivered!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return RedirectToAction(nameof(Index));

            var delivery = await _context.PrescriptionDeliveries
                .Include(d => d.Prescription)
                    .ThenInclude(p => p.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (delivery == null) return RedirectToAction(nameof(Index));
            return View(delivery);
        }

        // POST: DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var delivery = await _context.PrescriptionDeliveries.FindAsync(id);
            if (delivery != null)
            {
                _context.PrescriptionDeliveries.Remove(delivery);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Prescription delivery deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PrescriptionDeliveryExists(int id)
        {
            return _context.PrescriptionDeliveries.Any(e => e.Id == id);
        }
    }
}
