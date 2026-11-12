using HealthOps_Project.Data;
using HealthOps_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HealthOps_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index(string searchString, string role, string status)
        {
            var employees = _context.Employees.AsQueryable();

            // Search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e =>
                    e.FirstName.Contains(searchString) ||
                    e.LastName.Contains(searchString) ||
                    e.IdNumber.Contains(searchString) ||
                    e.DepartmentName.Contains(searchString) ||
                    e.WardName.Contains(searchString));
            }

            // Role filter (enum)
            if (!string.IsNullOrEmpty(role) && Enum.TryParse<EmployeeRoleType>(role, out EmployeeRoleType roleType))
            {
                employees = employees.Where(e => e.Role == roleType);
            }

            // Status filter
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<EmploymentStatus>(status, out EmploymentStatus employmentStatus))
            {
                employees = employees.Where(e => e.Status == employmentStatus);
            }

            ViewBag.SearchString = searchString;
            ViewBag.Role = role;
            ViewBag.Status = status;

            return View(await employees.OrderBy(e => e.LastName).ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return BadRequest();

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null) return NotFound();

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            // Remove validation for navigation properties that might be null
            ModelState.Remove("Department");
            ModelState.Remove("Ward");

            if (ModelState.IsValid)
            {
                employee.CreatedDate = DateTime.Now;

                // Ensure DepartmentId and WardId are null if not provided
                if (employee.DepartmentId == 0) employee.DepartmentId = null;
                if (employee.WardId == 0) employee.WardId = null;

                _context.Add(employee);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Employee created successfully.";
                return RedirectToAction(nameof(Index));
            }

            // Log validation errors for debugging
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                System.Diagnostics.Debug.WriteLine($"Validation Error: {error.ErrorMessage}");
            }

            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();

            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.EmployeeId) return BadRequest();

            // Remove validation for navigation properties
            ModelState.Remove("Department");
            ModelState.Remove("Ward");

            if (ModelState.IsValid)
            {
                try
                {
                    employee.ModifiedDate = DateTime.Now;

                    // Ensure DepartmentId and WardId are null if not provided
                    if (employee.DepartmentId == 0) employee.DepartmentId = null;
                    if (employee.WardId == 0) employee.WardId = null;

                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Employee updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await EmployeeExistsAsync(employee.EmployeeId))
                        return NotFound();
                    else
                        throw;
                }
            }

            // Log validation errors for debugging
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                System.Diagnostics.Debug.WriteLine($"Validation Error: {error.ErrorMessage}");
            }

            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null) return NotFound();

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Employee deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // Check if employee exists
        private async Task<bool> EmployeeExistsAsync(int id)
        {
            return await _context.Employees.AnyAsync(e => e.EmployeeId == id);
        }
    }
}