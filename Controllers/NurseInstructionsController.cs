using HealthOps_Project.Data;
using HealthOps_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HealthOps_Project.Controllers
{
    public class NurseInstructionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NurseInstructionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private IEnumerable<SelectListItem> GetPatientsSelectList()
        {
            return _context.Patients
                .Where(p => p.IsActive) // optional: only active patients
                .Select(p => new SelectListItem
                {
                    Value = p.PatientId.ToString(),

                    Text = p.SouthAfricanID + " " +
                    p.FirstName + " " + p.LastName
                })
                .ToList();
        }

        // Helper: Active instructions query
        private IQueryable<NurseInstruction> ActiveInstructions()
        {
            return _context.NurseInstructions
                .Include(n => n.Patient)
                .Where(n => n.isActive);
        }

        [Authorize(Roles = "Nurse , NursingSister")]
        public async Task<IActionResult> NurseDetails(int id)
        {
            var instruction = await _context.NurseInstructions
                .Include(n => n.Patient)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (instruction == null)
                return NotFound();

            return View("NurseDetails", instruction);
        }


        public async Task<IActionResult> NurseIndex()
        {
            var instructions = await _context.NurseInstructions
                .Include(n => n.Patient)
                .Where(n => !n.IsCompleted && n.isActive)
                .OrderByDescending(n => n.IssuedAt)
                .ToListAsync();

            // Check logged-in user roles
            if (User.IsInRole("Nurse"))
            {
                ViewData["DashboardUrl"] = Url.Action("Nurse", "Dashboard");
            }
            else if (User.IsInRole("NursingSister"))
            {
                ViewData["DashboardUrl"] = Url.Action("NursingSister", "Dashboard");
            }

            return View(instructions);
        }




        // GET: NurseInstructions
        public async Task<IActionResult> Index()
        {
            var instructions = await _context.NurseInstructions
                .Include(n => n.Patient)
                .Where(n => n.isActive == true)
                .OrderByDescending(n => n.IssuedAt)
                .ToListAsync();
            return View(instructions);
        }

        // GET: NurseInstructions/Details or NurseInstructions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            NurseInstruction nurseInstruction;

            if (id == null)
            {
                // If no id is given, show the most recent instruction
                nurseInstruction = await _context.NurseInstructions
                    .Include(n => n.Patient)
                    .OrderByDescending(n => n.IssuedAt)
                    .FirstOrDefaultAsync();
            }
            else
            {
                // Otherwise, find the one with that ID
                nurseInstruction = await _context.NurseInstructions
                    .Include(n => n.Patient)
                    .FirstOrDefaultAsync(m => m.Id == id);
            }

            if (nurseInstruction == null)
            {
                return NotFound();
            }

            return View(nurseInstruction);
        }



        // GET: NurseInstructions/Create
        public IActionResult Create()
        {
            ViewBag.PatientList = GetPatientsSelectList();
            return View();
        }

        // POST: NurseInstructions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,NurseName,Instruction,IsCompleted")] NurseInstruction nurseInstruction)
        {
            if (ModelState.IsValid)
            {
                nurseInstruction.IssuedAt = DateTime.UtcNow;
                _context.Add(nurseInstruction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.PatientList = GetPatientsSelectList();
            return View(nurseInstruction);
        }

        // GET: NurseInstructions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nurseInstruction = await _context.NurseInstructions.FindAsync(id);
            if (nurseInstruction == null)
            {
                return NotFound();
            }
            ViewBag.PatientList = GetPatientsSelectList();
            return View(nurseInstruction);
        }

        // POST: NurseInstructions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PatientId,NurseName,Instruction,IssuedAt,IsCompleted")] NurseInstruction nurseInstruction)
        {
            if (id != nurseInstruction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nurseInstruction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NurseInstructionExists(nurseInstruction.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.PatientList = GetPatientsSelectList();
            return View(nurseInstruction);
        }

        // GET: NurseInstructions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nurseInstruction = await _context.NurseInstructions
                .Include(n => n.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (nurseInstruction == null)
            {
                return NotFound();
            }

            return View(nurseInstruction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var nurseInstruction = await _context.NurseInstructions.FindAsync(id);
            if (nurseInstruction != null)
            {
                nurseInstruction.isActive = false; // soft delete
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool NurseInstructionExists(int id)
        {
            return _context.NurseInstructions.Any(e => e.Id == id);
        }
    }
}