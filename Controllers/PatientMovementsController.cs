using HealthOps_Project.Data;
using HealthOps_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HealthOps_Project.Controllers
{
    public class PatientMovementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientMovementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Index
        public async Task<IActionResult> Index()
        {
            var movements = await _context.PatientMovements
                .Include(m => m.Patient)
                .Include(m => m.Ward)
                .Include(m => m.Room)
                .Include(m => m.Bed)
                .Include(m => m.DoctorUser)
                .Where(m => m.IsActive)
                .OrderByDescending(m => m.MovementDate)
                .ToListAsync();

            return View(movements);
        }

        // GET: Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var movement = await _context.PatientMovements
                .Include(m => m.Patient)
                .Include(m => m.Ward)
                .Include(m => m.Room)
                .Include(m => m.Bed)
                .Include(m => m.DoctorUser)
                .FirstOrDefaultAsync(m => m.MovementId == id);

            if (movement == null) return NotFound();
            return View(movement);
        }

        // GET: PatientMovements/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        // POST: PatientMovements/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientMovement movement)
        {
            try
            {
                // Ensure movement date is set
                if (movement.MovementDate == default)
                {
                    movement.MovementDate = DateTime.Now;
                }

                // Get the patient's current active movement
                var currentMovement = await _context.PatientMovements
                    .Include(m => m.Bed)
                    .Where(m => m.PatientId == movement.PatientId && m.IsActive)
                    .OrderByDescending(m => m.MovementDate)
                    .FirstOrDefaultAsync();

                // Get the patient's current active admission
                var currentAdmission = await _context.Admissions
                    .Include(a => a.Ward)
                    .Include(a => a.Bed)
                    .Where(a => a.PatientId == movement.PatientId && a.IsActive)
                    .OrderByDescending(a => a.AdmissionDate)
                    .FirstOrDefaultAsync();

                // If patient has a current movement, deactivate it and free up the bed
                if (currentMovement != null)
                {
                    currentMovement.IsActive = false;

                    // Mark the previous bed as available
                    if (currentMovement.Bed != null)
                    {
                        currentMovement.Bed.IsAvailable = true;
                        _context.Update(currentMovement.Bed);
                    }

                    _context.Update(currentMovement);
                }

                // If patient has a current admission, update it to the new ward and bed
                if (currentAdmission != null)
                {
                    // Update admission to new ward and bed
                    currentAdmission.WardId = movement.WardId;
                    currentAdmission.BedId = movement.BedId;
                    _context.Update(currentAdmission);
                }
                else
                {
                    // If no active admission exists, create one
                    var admission = new Admission
                    {
                        PatientId = movement.PatientId,
                        WardId = movement.WardId,
                        BedId = movement.BedId,
                        AdmissionDate = movement.MovementDate,
                        IsActive = true,
                    };
                    _context.Add(admission);
                }

                // Mark the new bed as occupied
                var newBed = await _context.Beds.FindAsync(movement.BedId);
                if (newBed != null)
                {
                    newBed.IsAvailable = false;
                    _context.Update(newBed);
                }

                // Set the new movement as active
                movement.IsActive = true;
                _context.Add(movement);

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Patient moved successfully to {movement.Ward?.Name}!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Could not create patient movement. Please try again.");
                Console.WriteLine($"Error creating patient movement: {ex.Message}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }

            // Repopulate dropdowns and return to view with errors
            PopulateDropdowns();
            return View(movement);
        }

        // GET: PatientMovements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var movement = await _context.PatientMovements
                .Include(m => m.Patient)
                .Include(m => m.Ward)
                .Include(m => m.Room)
                .Include(m => m.Bed)
                .FirstOrDefaultAsync(m => m.MovementId == id);

            if (movement == null || !movement.IsActive) return NotFound();

            PopulateDropdowns();
            return View(movement);
        }

        // POST: PatientMovements/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PatientMovement movement)
        {
            if (id != movement.MovementId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the original movement before update
                    var originalMovement = await _context.PatientMovements
                        .AsNoTracking()
                        .FirstOrDefaultAsync(m => m.MovementId == id);

                    if (originalMovement == null) return NotFound();

                    // Get the patient's current active admission
                    var currentAdmission = await _context.Admissions
                        .Include(a => a.Ward)
                        .Where(a => a.PatientId == movement.PatientId && a.IsActive)
                        .OrderByDescending(a => a.AdmissionDate)
                        .FirstOrDefaultAsync();

                    // If bed changed, update bed availability and admission
                    if (originalMovement.BedId != movement.BedId || originalMovement.WardId != movement.WardId)
                    {
                        // Free up the old bed
                        var oldBed = await _context.Beds.FindAsync(originalMovement.BedId);
                        if (oldBed != null)
                        {
                            oldBed.IsAvailable = true;
                            _context.Update(oldBed);
                        }

                        // Occupy the new bed
                        var newBed = await _context.Beds.FindAsync(movement.BedId);
                        if (newBed != null)
                        {
                            newBed.IsAvailable = false;
                            _context.Update(newBed);
                        }

                        // Update admission if ward or bed changed
                        if (currentAdmission != null)
                        {
                            var previousWardName = currentAdmission.Ward?.Name;

                            currentAdmission.WardId = movement.WardId;
                            currentAdmission.BedId = movement.BedId;

                            // Add movement details to admission notes
                            //if (!string.IsNullOrEmpty(currentAdmission.Notes))
                            //{
                            //    currentAdmission.Notes += $"\n[{movement.MovementDate:MM/dd/yyyy HH:mm}] Patient moved from {previousWardName} to {movement.Ward?.Name} - Bed {movement.Bed?.BedNumber} (Updated)";
                            //}
                            //else
                            //{
                            //    currentAdmission.Notes = $"[{movement.MovementDate:MM/dd/yyyy HH:mm}] Patient moved from {previousWardName} to {movement.Ward?.Name} - Bed {movement.Bed?.BedNumber} (Updated)";
                            //}

                            _context.Update(currentAdmission);
                        }
                    }

                    _context.Update(movement);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Patient movement updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.PatientMovements.Any(p => p.MovementId == id))
                        return NotFound();
                    else
                        throw;
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Could not update the movement. Please check your data and try again.");
                }
            }

            PopulateDropdowns();
            return View(movement);
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var movement = await _context.PatientMovements
                .Include(m => m.Patient)
                .Include(m => m.Ward)
                .Include(m => m.Room)
                .Include(m => m.Bed)
                .FirstOrDefaultAsync(m => m.MovementId == id);

            if (movement == null) return NotFound();
            return View(movement);
        }

        // POST: Delete (Soft delete)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movement = await _context.PatientMovements.FindAsync(id);
            if (movement != null)
            {
                movement.IsActive = false;

                // Free up the bed when movement is deleted
                var bed = await _context.Beds.FindAsync(movement.BedId);
                if (bed != null)
                {
                    bed.IsAvailable = true;
                    _context.Update(bed);
                }

                // Get the patient's current active admission to revert if needed
                var currentAdmission = await _context.Admissions
                    .Where(a => a.PatientId == movement.PatientId && a.IsActive)
                    .OrderByDescending(a => a.AdmissionDate)
                    .FirstOrDefaultAsync();

                // Optionally: You might want to revert the admission to previous state
                // This would require storing previous ward/bed information

                _context.Update(movement);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Patient movement deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // AJAX endpoint to get current patient location and admission info
        public async Task<JsonResult> GetCurrentLocation(int patientId)
        {
            var currentMovement = await _context.PatientMovements
                .Include(m => m.Ward)
                .Include(m => m.Room)
                .Include(m => m.Bed)
                .Where(m => m.PatientId == patientId && m.IsActive)
                .OrderByDescending(m => m.MovementDate)
                .FirstOrDefaultAsync();

            var currentAdmission = await _context.Admissions
                .Include(a => a.Ward)
                .Include(a => a.Bed)
                .Where(a => a.PatientId == patientId && a.IsActive)
                .OrderByDescending(a => a.AdmissionDate)
                .FirstOrDefaultAsync();

            if (currentMovement != null && currentAdmission != null)
            {
                return Json(new
                {
                    exists = true,
                    currentWard = currentAdmission.Ward?.Name,
                    currentBed = currentAdmission.Bed?.BedNumber,
                    movementWard = currentMovement.Ward?.Name,
                    movementBed = currentMovement.Bed?.BedNumber,
                    room = currentMovement.Room?.RoomNumber
                });
            }

            return Json(new { exists = false });
        }

        // AJAX endpoint to get beds for a specific ward
        public async Task<JsonResult> GetBedsByWard(int wardId)
        {
            var beds = await _context.Beds
                .Where(b => b.WardId == wardId && b.IsAvailable)
                .Select(b => new { b.BedId, b.BedNumber })
                .ToListAsync();

            return Json(beds);
        }

        // Helpers
        private void PopulateDropdowns()
        {
            ViewBag.Patients = new SelectList(_context.Patients.Where(p => p.IsActive).ToList(), "PatientId", "FirstName");
            ViewBag.Wards = new SelectList(_context.Wards.Where(w => w.IsActive).ToList(), "WardId", "Name");
            ViewBag.Rooms = new SelectList(_context.Rooms.Where(r => r.IsAvailable).ToList(), "RoomId", "RoomNumber");
            ViewBag.Beds = new SelectList(_context.Beds.Where(b => b.IsAvailable).ToList(), "BedId", "BedNumber");
            ViewBag.Doctors = new SelectList(_context.Users.ToList(), "Id", "UserName");
            // In your controller
            ViewBag.DoctorId = new SelectList(
                _context.Doctors
                    .Where(d => d.IsActive)
                    .Select(d => new {
                        d.DoctorId,
                        FullName = d.FirstName + " " + d.LastName
                    }),
                "DoctorId",
                "FullName"
            );
        }
    }
}