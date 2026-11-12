using HealthOps_Project.Data;
using HealthOps_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthOps_Project.Controllers
{
    public class AdmissionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdmissionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admissions
        public async Task<IActionResult> Index()
        {
            var admissions = await _context.Admissions
                .Include(a => a.Patient)
                .Include(a => a.Ward)
                .Include(a => a.Bed)
                .Include(a => a.Doctor)
                .Where(a => a.IsActive)
                .ToListAsync();

            return View(admissions);
        }

        // GET: Admissions/LiveSearch
        public async Task<IActionResult> LiveSearch(string query)
        {
            var admissionsQuery = _context.Admissions
                .Include(a => a.Patient)
                .Include(a => a.Ward)
                .Include(a => a.Bed)
                .Include(a => a.Doctor)
                .Where(a => a.IsActive);

            if (!string.IsNullOrEmpty(query))
            {
                admissionsQuery = admissionsQuery
                    .Where(a => a.Patient.FirstName.Contains(query) ||
                                a.Patient.LastName.Contains(query) ||
                                a.Doctor.FirstName.Contains(query) ||
                                a.Doctor.LastName.Contains(query));
            }

            var admissions = await admissionsQuery.ToListAsync();

            string rowsHtml = "";
            foreach (var a in admissions)
            {
                rowsHtml += "<tr class='align-middle text-center'>";
                rowsHtml += $"<td>{a.Patient?.FirstName} {a.Patient?.LastName}</td>";
                rowsHtml += $"<td>{a.Doctor?.FirstName} {a.Doctor?.LastName}</td>";
                rowsHtml += $"<td>{a.Ward?.Name}</td>";
                rowsHtml += $"<td>{a.Bed?.BedNumber}</td>";
                rowsHtml += $"<td>{a.AdmissionDate.ToShortDateString()}</td>";
                rowsHtml += $"<td>{a.DischargeDate?.ToShortDateString()}</td>";
                rowsHtml += "<td>";
                rowsHtml += $"<a href='/Admissions/Edit/{a.Id}' class='btn btn-sm btn-warning me-1'>✏️</a>";
                rowsHtml += $"<a href='/Admissions/Details/{a.Id}' class='btn btn-sm btn-info me-1'>🔍</a>";
                rowsHtml += $"<a href='/Admissions/Delete/{a.Id}' class='btn btn-sm btn-danger'>🗑️</a>";
                rowsHtml += "</td></tr>";
            }

            return Content(rowsHtml, "text/html");
        }

        // GET: Admissions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var admission = await _context.Admissions
                .Include(a => a.Patient)
                .Include(a => a.Ward)
                .Include(a => a.Bed)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (admission == null) return NotFound();

            return View(admission);
        }
        // GET: Admissions/Create
        public IActionResult Create()
        {
            ViewBag.PatientId = new SelectList(
                _context.Patients.Where(p => p.IsActive)
                .Select(p => new
                {
                    p.PatientId,
                    FullName = p.FirstName + " " + p.LastName
                }), "PatientId", "FullName");

            ViewBag.WardId = new SelectList(_context.Wards, "WardId", "Name");
            ViewBag.BedId = new SelectList(_context.Beds, "BedId", "BedNumber");

            ViewBag.DoctorId = new SelectList(
                _context.Doctors.Where(d => d.IsActive)
                .Select(d => new
                {
                    d.DoctorId,
                    FullName = d.FirstName + " " + d.LastName
                }), "DoctorId", "FullName");

            return View();
        }
        // POST: Admissions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Admission admission)
        {
           
                try
                {
                    admission.IsActive = true;
                    _context.Add(admission);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "✅ Patient admitted and doctor assigned successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "❌ Could not complete the admission. Try again.");
                }
           

            ViewBag.PatientId = new SelectList(_context.Patients.Where(p => p.IsActive).ToList(), "PatientId", "FirstName", admission.PatientId);
            ViewBag.WardId = new SelectList(_context.Wards.ToList(), "WardId", "Name", admission.WardId);
            ViewBag.BedId = new SelectList(_context.Beds.ToList(), "BedId", "BedNumber", admission.BedId);
            ViewBag.DoctorId = new SelectList(_context.Doctors.ToList(), "DoctorId", "FirstName", admission.DoctorId);

            return View(admission);
        }

        // GET: Admissions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var admission = await _context.Admissions.FindAsync(id);
            if (admission == null) return NotFound();

            // Get the current doctor's name
            var currentDoctor = await _context.Doctors.FindAsync(admission.DoctorId);
            ViewBag.CurrentDoctorName = currentDoctor != null ? $"{currentDoctor.FirstName} {currentDoctor.LastName}" : "Not assigned";

            await PopulateViewBags(admission);
            return View(admission);
        }

        // POST: Admissions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Admission admission)
        {
            if (id != admission.Id) return NotFound();

                try
                {
                    _context.Update(admission);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "✅ Admission updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Admissions.Any(a => a.Id == id))
                        return NotFound();
                    else
                        throw;
                }
            

            ViewBag.PatientId = new SelectList(_context.Patients.Where(p => p.IsActive).ToList(), "PatientId", "FirstName", admission.PatientId);
            ViewBag.WardId = new SelectList(_context.Wards.ToList(), "WardId", "Name", admission.WardId);
            ViewBag.BedId = new SelectList(_context.Beds.ToList(), "BedId", "BedNumber", admission.BedId);
            ViewBag.DoctorId = new SelectList(_context.Doctors.ToList(), "DoctorId", "FirstName", admission.DoctorId);

            return View(admission);
        }

        // Helper method to populate ViewBags
        private async Task PopulateViewBags(Admission admission)
        {
            ViewBag.PatientId = new SelectList(
                await _context.Patients.Where(p => p.IsActive)
                    .Select(p => new { p.PatientId, FullName = p.FirstName + " " + p.LastName })
                    .ToListAsync(),
                "PatientId", "FullName", admission.PatientId);

            ViewBag.WardId = new SelectList(
                await _context.Wards.ToListAsync(),
                "WardId", "Name", admission.WardId);

            ViewBag.BedId = new SelectList(
                await _context.Beds.ToListAsync(),
                "BedId", "BedNumber", admission.BedId);

            ViewBag.DoctorId = new SelectList(
                await _context.Doctors.Where(d => d.IsActive)
                    .Select(d => new { d.DoctorId, FullName = d.FirstName + " " + d.LastName })
                    .ToListAsync(),
                "DoctorId", "FullName", admission.DoctorId);
        }

        // Soft Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var admission = await _context.Admissions.FindAsync(id);
            if (admission == null) return NotFound();

            admission.IsActive = false;
            _context.Update(admission);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "🟠 Admission marked as inactive.";
            return RedirectToAction(nameof(Index));
        }
    }
}
