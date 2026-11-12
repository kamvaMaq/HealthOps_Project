using HealthOps_Project.Data;
using HealthOps_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthOps_Project.Controllers
{
    public class DischargesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DischargesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Discharges
        public async Task<IActionResult> Index()
        {
            var discharges = await _context.Discharges
                .Include(d => d.Patient)
                .Include(d => d.Ward)
                .Where(d => d.IsActive)
                .ToListAsync();

            return View(discharges);
        }

        // GET: Discharges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var discharge = await _context.Discharges
                .Include(d => d.Patient)
                .Include(d => d.Ward)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (discharge == null) return NotFound();

            return View(discharge);
        }

        // GET: Discharges/Create
        public IActionResult Create()
        {
            // Combine FirstName and LastName for display
            var patientList = _context.Patients
                .Where(p => p.IsActive)
                .Select(p => new
                {
                    p.PatientId,
                    FullName = p.FirstName + " " + p.LastName
                })
                .ToList();

            ViewBag.PatientList = new SelectList(
                patientList,
                "PatientId",
                "FullName"
            );

            ViewBag.WardList = new SelectList(
                _context.Wards.ToList(),
                "WardId",
                "Name"
            );

            return View();
        }



        // POST: Discharges/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Discharge discharge)
        {

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Add the discharge
                _context.Discharges.Add(discharge);

                // Find the active admission for this patient
                var admission = await _context.Admissions
                    .Where(a => a.PatientId == discharge.PatientId && a.IsActive)
                    .FirstOrDefaultAsync();

                if (admission != null)
                {
                    // Soft delete or mark inactive
                    admission.IsActive = false;
                    _context.Admissions.Update(admission);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "✅ Discharge recorded and admission closed successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", $"Error saving discharge: {ex.Message}");
            }


            // Repopulate dropdowns if there is an error
            ViewBag.PatientList = new SelectList(_context.Patients.Where(p => p.IsActive).ToList(), "PatientId", "FirstName", discharge.PatientId);
            ViewBag.WardList = new SelectList(_context.Wards.ToList(), "WardId", "Name", discharge.WardId);

            return View(discharge);
        }




        // POST: Discharges/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Discharge discharge)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            discharge.IsActive = true; // Ensure it’s active by default
        //            _context.Discharges.Add(discharge);
        //            await _context.SaveChangesAsync();

        //            TempData["SuccessMessage"] = "✅ Discharge recorded successfully!";
        //            return RedirectToAction(nameof(Index));
        //        }
        //        catch (Exception ex)
        //        {
        //            ModelState.AddModelError("", $"Error saving discharge: {ex.Message}");
        //        }
        //    }

        //    ViewBag.PatientList = new SelectList(
        //        _context.Patients.Where(p => p.IsActive).ToList(),
        //        "PatientId",
        //        "FirstName",
        //        discharge.PatientId
        //    );

        //    ViewBag.WardList = new SelectList(
        //        _context.Wards.ToList(),
        //        "WardId",
        //        "Name",
        //        discharge.WardId
        //    );

        //    return View(discharge);
        //}
        // GET: Discharges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var discharge = await _context.Discharges.FindAsync(id);
            if (discharge == null) return NotFound();

            // Combine FirstName and LastName for display
            var patientList = _context.Patients
                .Where(p => p.IsActive)
                .Select(p => new
                {
                    p.PatientId,
                    FullName = p.FirstName + " " + p.LastName
                })
                .ToList();

            ViewBag.PatientList = new SelectList(
                patientList,
                "PatientId",
                "FullName",
                discharge.PatientId
            );

            ViewBag.WardList = new SelectList(
                _context.Wards.ToList(),
                "WardId",
                "Name",
                discharge.WardId
            );

            return View(discharge);
        }


        // POST: Discharges/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Discharge discharge)
        {
            if (id != discharge.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(discharge);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "✅ Discharge updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Discharges.Any(d => d.Id == id))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewBag.PatientList = new SelectList(
                _context.Patients.Where(p => p.IsActive).ToList(),
                "PatientId",
                "FirstName",
                discharge.PatientId
            );

            ViewBag.WardList = new SelectList(
                _context.Wards.ToList(),
                "WardId",
                "Name",
                discharge.WardId
            );

            return View(discharge);
        }

        // GET: Discharges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var discharge = await _context.Discharges
                .Include(d => d.Patient)
                .Include(d => d.Ward)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (discharge == null) return NotFound();

            return View(discharge);
        }

        // POST: Discharges/Delete (Soft Delete)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discharge = await _context.Discharges.FindAsync(id);
            if (discharge == null) return NotFound();

            discharge.IsActive = false;
            _context.Update(discharge);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "🟠 Discharge has been marked as inactive (soft deleted).";
            TempData["UndoDischargeId"] = discharge.Id;

            return RedirectToAction(nameof(Index));
        }

        // Undo soft delete
        public async Task<IActionResult> UndoDelete(int id)
        {
            var discharge = await _context.Discharges.FindAsync(id);
            if (discharge != null)
            {
                discharge.IsActive = true;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "✅ Discharge restored successfully!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
