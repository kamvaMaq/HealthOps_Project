using HealthOps_Project.Data;
using HealthOps_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthOps_Project.Controllers
{
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var patients = await _context.Patients.ToListAsync();
            return View(patients); // looks for Views/Patients/Index.cshtml
        }


        // GET: Patients
        //public async Task<IActionResult> Index(string searchString)
        //{
        //    var patients = from p in _context.Patients
        //                   select p;

        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        patients = patients.Where(p => p.FirstName.Contains(searchString)
        //                                    || p.LastName.Contains(searchString));
        //    }

        //    return View(await patients.ToListAsync());
        //}

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients.FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null) return NotFound();

            return View(patient);
        }

        // Employees Create
        public IActionResult Create()
        {
            ViewBag.EmployeeList = new SelectList(_context.Patients, "DepartmentID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient department)
        {
            try
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Patient has been added successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Could not perform action please try again...");
            }

            ViewBag.EmployeeList = new SelectList(_context.Doctors, "DoctorId", "FirstName", department.FirstName);

            return View(department);
        }

        // Departments Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Patient employee)
        {
            if (id != employee.Id)
                return NotFound();


            try
            {
                _context.Update(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Patients.Any(p => p.Id == id))
                    return NotFound();
                else
                    throw;
            }


            ViewBag.EmployeeList = new SelectList(_context.Patients, "DoctorID", "FirstName", employee.FirstName);
            return View(employee);
        }

        // GET: Patients/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null) return NotFound();

        //    var patient = await _context.Patients.FirstOrDefaultAsync(m => m.PatientId == id);
        //    if (patient == null) return NotFound();

        //    return View(patient);
        //}

        //// POST: Patients/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var patient = await _context.Patients.FindAsync(id);
        //    _context.Patients.Remove(patient);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}



        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients.FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null) return NotFound();

            return View(patient);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            // ✅ Soft delete
            patient.IsActive = false;
            _context.Update(patient);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }





        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }
    }
}
