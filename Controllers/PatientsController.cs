using HealthOps_Project.Data;
using HealthOps_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthOps_Project.Controllers
{
    //[Authorize(Roles = "Admin,Doctor,WardAdmin")]
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PatientsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Patients/Index
        public async Task<IActionResult> Index()
        {
            var list = await _db.Patients
                .Include(p => p.Bed)
                .Where(n => n.IsActive == true)
                .ToListAsync();
            return View(list);
        }

        // GET: Patients/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.BedList = new SelectList(await _db.Beds.ToListAsync(), "BedId", "BedNumber");
            return View();
        }

        // GET: Patients/PatientIndex
        public async Task<IActionResult> PatientIndex(string query)
        {
            IQueryable<Patient> patientsQuery = _db.Patients.Where(p => p.IsActive == true);

            if (!string.IsNullOrEmpty(query))
            {
                patientsQuery = patientsQuery.Where(p =>
                    p.PatientId.ToString().Contains(query) ||
                    p.FirstName.Contains(query) ||
                    p.LastName.Contains(query)
                );
            }

            var patients = await patientsQuery.ToListAsync();
            return View(patients);
        }

        
        public async Task<IActionResult> PatientDetails(int id)
        {
            var patient = await _db.Patients
                .Include(p => p.Bed)
                .FirstOrDefaultAsync(m => m.PatientId == id);

            if (patient == null)
                return NotFound();

            return View(patient);
        }

        // GET: Patients/Search
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                var patients = await _db.Patients.ToListAsync();
                return View("Index", patients);
            }

            var filteredPatients = await _db.Patients
                .Where(p => p.PatientId.ToString().Contains(query) ||
                           p.FirstName.Contains(query) ||
                           p.LastName.Contains(query))
                .ToListAsync();

            return View("Index", filteredPatients);
        }

        // GET: Patients/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return NotFound();

            var patient = await _db.Patients
                .Include(p => p.Bed)
                .FirstOrDefaultAsync(m => m.PatientId == id);

            if (patient == null)
                return NotFound();

            return View(patient);
        }

        // POST: Patients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient)
        {
            patient.AdmissionDate = DateTime.Now;
            patient.IsActive = true;

            if (!ModelState.IsValid)
            {
                ViewBag.BedList = new SelectList(await _db.Beds.ToListAsync(), "BedId", "BedNumber", patient.BedId);
                return View(patient);
            }

            if (await _db.Patients.AnyAsync(p => p.SouthAfricanID == patient.SouthAfricanID))
            {
                ModelState.AddModelError("SouthAfricanID", "This ID already exists.");
                ViewBag.BedList = new SelectList(await _db.Beds.ToListAsync(), "BedId", "BedNumber", patient.BedId);
                return View(patient);
            }

            if (patient.BedId.HasValue && !await _db.Beds.AnyAsync(b => b.BedId == patient.BedId.Value))
            {
                ModelState.AddModelError("BedId", "Selected bed does not exist.");
                ViewBag.BedList = new SelectList(await _db.Beds.ToListAsync(), "BedId", "BedNumber", patient.BedId);
                return View(patient);
            }

            _db.Patients.Add(patient);
            await _db.SaveChangesAsync();

            if (User.IsInRole("Doctor"))
            {
                return RedirectToAction("PatientIndex");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Patients/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return NotFound();

            var patient = await _db.Patients
                .Include(p => p.Bed)
                .FirstOrDefaultAsync(p => p.PatientId == id);

            if (patient == null)
                return NotFound();

            ViewBag.BedList = new SelectList(await _db.Beds.ToListAsync(), "BedId", "BedNumber", patient.BedId);
            return View(patient);
        }

        // POST: Patients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Patient patient)
        {
            if (id != patient.PatientId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.BedList = new SelectList(await _db.Beds.ToListAsync(), "BedId", "BedNumber", patient.BedId);
                return View(patient);
            }

            if (await _db.Patients.AnyAsync(p => p.SouthAfricanID == patient.SouthAfricanID && p.PatientId != patient.PatientId))
            {
                ModelState.AddModelError("SouthAfricanID", "This South African ID already exists.");
                ViewBag.BedList = new SelectList(await _db.Beds.ToListAsync(), "BedId", "BedNumber", patient.BedId);
                return View(patient);
            }

            if (patient.BedId.HasValue && !await _db.Beds.AnyAsync(b => b.BedId == patient.BedId.Value))
            {
                ModelState.AddModelError("BedId", "Selected bed does not exist.");
                ViewBag.BedList = new SelectList(await _db.Beds.ToListAsync(), "BedId", "BedNumber", patient.BedId);
                return View(patient);
            }

            try
            {
                patient.IsActive = true;
                _db.Update(patient);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _db.Patients.AnyAsync(e => e.PatientId == patient.PatientId))
                    return NotFound();
                else
                    throw;
            }

            if (User.IsInRole("Doctor"))
            {
                return RedirectToAction("PatientIndex");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Patients/Discharge/5
        [HttpGet]
        public async Task<IActionResult> Discharge(int id)
        {
            var patient = await _db.Patients.FindAsync(id);
            if (patient == null)
                return NotFound();

            return View(patient);
        }

        // POST: Patients/Discharge/5
        [HttpPost, ActionName("Discharge")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DischargeConfirmed(int id)
        {
            var patient = await _db.Patients.FindAsync(id);
            if (patient != null)
            {
                patient.IsActive = false;
                await _db.SaveChangesAsync();
            }

            if (User.IsInRole("Doctor"))
            {
                return RedirectToAction("PatientIndex");
            }
            else
            {
                return RedirectToAction("Index");
            }

        }


    }
}
