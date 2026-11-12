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
    public class DoctorInstructionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorInstructionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper: get patient dropdown
        private async Task<IEnumerable<SelectListItem>> GetPatientsSelectList()
        {
            return await _context.Patients
                .Where(p => p.IsActive)
                .Select(p => new SelectListItem
                {
                    Value = p.PatientId.ToString(),
                    Text = p.SouthAfricanID + " - " + p.FirstName + " " + p.LastName
                })
                .ToListAsync();
        }

        // GET: DoctorInstructions
        public async Task<IActionResult> Index()
        {
            var instructions = await _context.DoctorInstructions
                .Include(d => d.Patient)
                .OrderByDescending(d => d.CreatedDate)
                .ToListAsync();

            return View(instructions);
        }


        // GET: DoctorInstructions
        public async Task<IActionResult> DoctorIndex()
        {
            var instructions = await _context.DoctorInstructions
                .Include(d => d.Patient)
                .OrderByDescending(d => d.CreatedDate)
                .ToListAsync();

            return View(instructions);
        }
        // GET: DoctorInstructions/Details/5
        [Authorize(Roles = "Doctor,Nurse")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var instruction = await _context.DoctorInstructions
                .Include(d => d.Patient)
                .FirstOrDefaultAsync(d => d.DoctorInstructionId == id);

            if (instruction == null) return NotFound();

            return View(instruction);
        }


        // GET: DoctorInstructions/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.PatientList = await GetPatientsSelectList();
            return View();
        }

        // POST: DoctorInstructions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorInstruction instruction)
        {
            // Remove Patient navigation property from validation
            ModelState.Remove("Patient");

            if (ModelState.IsValid)
            {
                try
                {
                    instruction.CreatedDate = DateTime.Now;
                    _context.Add(instruction);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Doctor instruction created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error saving: {ex.Message}");
                }
            }

            // Log validation errors for debugging
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    System.Diagnostics.Debug.WriteLine($"Key: {state.Key}, Error: {error.ErrorMessage}");
                }
            }

            ViewBag.PatientList = await GetPatientsSelectList();
            return View(instruction);
        }

        // GET: DoctorInstructions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var instruction = await _context.DoctorInstructions.FindAsync(id);
            if (instruction == null) return NotFound();

            ViewBag.PatientList = await GetPatientsSelectList();
            return View(instruction);
        }

        // POST: DoctorInstructions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DoctorInstruction instruction)
        {
            if (id != instruction.DoctorInstructionId) return NotFound();

            // Remove Patient navigation property from validation
            ModelState.Remove("Patient");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instruction);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Doctor instruction updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorInstructionExists(instruction.DoctorInstructionId))
                        return NotFound();
                    else
                        throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating: {ex.Message}");
                }
            }

            // Log validation errors for debugging
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    System.Diagnostics.Debug.WriteLine($"Key: {state.Key}, Error: {error.ErrorMessage}");
                }
            }

            ViewBag.PatientList = await GetPatientsSelectList();
            return View(instruction);
        }


        // GET: DoctorInstructions/DocEdit/5
        public async Task<IActionResult> DocEdit(int? id)
        {
            if (id == null) return NotFound();

            var instruction = await _context.DoctorInstructions.FindAsync(id);
            if (instruction == null) return NotFound();

            ViewBag.PatientList = await GetPatientsSelectList();
            return View(instruction);
        }

        // POST: DoctorInstructions/DocEdit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DocEdit(int id, DoctorInstruction instruction)
        {
            if (id != instruction.DoctorInstructionId) return NotFound();

            // Only update the IsCompleted field to avoid EF errors
            var dbInstruction = await _context.DoctorInstructions.FindAsync(id);
            if (dbInstruction == null) return NotFound();

            dbInstruction.IsCompleted = instruction.IsCompleted;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Doctor instruction updated successfully!";
                return RedirectToAction("DoctorIndex"); // redirect to DoctorIndex
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorInstructionExists(id))
                    return NotFound();
                else
                    throw;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating: {ex.Message} - {ex.InnerException?.Message}");
            }

            // If we reach here, redisplay the form
            ViewBag.PatientList = await GetPatientsSelectList();
            return View(dbInstruction);
        }


        // GET: DoctorInstructions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var instruction = await _context.DoctorInstructions
                .Include(d => d.Patient)
                .FirstOrDefaultAsync(d => d.DoctorInstructionId == id);

            if (instruction == null) return NotFound();

            return View(instruction);
        }

        // POST: DoctorInstructions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instruction = await _context.DoctorInstructions.FindAsync(id);
            if (instruction != null)
            {
                instruction.IsCompleted = true;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Doctor instruction marked as completed!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorInstructionExists(int id)
        {
            return _context.DoctorInstructions.Any(e => e.DoctorInstructionId == id);
        }
    }
}
