using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthOps_Project.Data;
using HealthOps_Project.Models;

namespace HealthOps_Project.Controllers
{
    public class NursingSisterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NursingSisterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: NursingSister/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var patients = await _context.Patients
                .Include(p => p.Prescriptions)
                .Where(p => p.Prescriptions.Any(pr => pr.EndDate == null || pr.EndDate >= DateTime.UtcNow))
                .ToListAsync();


            return View("~/Views/Dashboard/NursingSister.cshtml", patients);
           
        }
     
    }
}
