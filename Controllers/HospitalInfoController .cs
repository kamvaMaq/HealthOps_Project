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
    public class HospitalInfoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HospitalInfoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HospitalInfo
        public async Task<IActionResult> Index()
        {
            var hospitalInfo = await _context.HospitalInfo
                .Include(h => h.InsurancePartnerships)
                .FirstOrDefaultAsync();

            if (hospitalInfo == null)
            {
                return RedirectToAction(nameof(Create));
            }

            return View(hospitalInfo);
        }

        // GET: HospitalInfo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var hospitalInfo = await _context.HospitalInfo
                .Include(h => h.InsurancePartnerships)
                .FirstOrDefaultAsync(h => h.HospitalId == id);

            if (hospitalInfo == null)
                return NotFound();

            return View(hospitalInfo);
        }

        // GET: HospitalInfo/Create
        public async Task<IActionResult> Create()
        {
            if (await _context.HospitalInfo.AnyAsync())
            {
                TempData["ErrorMessage"] = "Hospital information already exists. Please edit the existing record.";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        // POST: HospitalInfo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HospitalInfo hospitalInfo)
        {
            // Remove validation for navigation properties and system-managed fields
            ModelState.Remove("InsurancePartnerships");
            ModelState.Remove("CreatedDate");
            ModelState.Remove("ModifiedDate");

            // Remove validation for optional fields not in the form
            ModelState.Remove("BankName");
            ModelState.Remove("TaxNumber");
            ModelState.Remove("BranchCode");
            ModelState.Remove("AccountNumber");
            ModelState.Remove("AccreditationBody");
            ModelState.Remove("HealthFacilityCode");
            ModelState.Remove("RegistrationNumber");
            ModelState.Remove("AccreditationDate");

            // Set default values for required fields if empty
            if (string.IsNullOrWhiteSpace(hospitalInfo.OperatingHours))
            {
                hospitalInfo.OperatingHours = "24/7";
            }

            // Set empty strings for fields that might be required in DB but not in form
            if (string.IsNullOrWhiteSpace(hospitalInfo.BankName))
                hospitalInfo.BankName = string.Empty;
            if (string.IsNullOrWhiteSpace(hospitalInfo.TaxNumber))
                hospitalInfo.TaxNumber = string.Empty;
            if (string.IsNullOrWhiteSpace(hospitalInfo.BranchCode))
                hospitalInfo.BranchCode = string.Empty;
            if (string.IsNullOrWhiteSpace(hospitalInfo.AccountNumber))
                hospitalInfo.AccountNumber = string.Empty;
            if (string.IsNullOrWhiteSpace(hospitalInfo.AccreditationBody))
                hospitalInfo.AccreditationBody = string.Empty;
            if (string.IsNullOrWhiteSpace(hospitalInfo.HealthFacilityCode))
                hospitalInfo.HealthFacilityCode = string.Empty;
            if (string.IsNullOrWhiteSpace(hospitalInfo.RegistrationNumber))
                hospitalInfo.RegistrationNumber = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    hospitalInfo.CreatedDate = DateTime.Now;
                    hospitalInfo.ModifiedDate = null;

                    _context.Add(hospitalInfo);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Hospital information created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error saving hospital: {ex.Message}");
                    TempData["ErrorMessage"] = $"Error creating hospital: {ex.Message}";
                }
            }

            // Debug: Log validation errors
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                System.Diagnostics.Debug.WriteLine($"Validation Error: {error.ErrorMessage}");
            }

            // Show validation errors to user
            if (!ModelState.IsValid)
            {
                var errorMessages = string.Join("; ", ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => $"{x.Key}: {string.Join(", ", x.Value.Errors.Select(e => e.ErrorMessage))}"));
                TempData["ErrorMessage"] = $"Please correct the following errors: {errorMessages}";
            }

            return View(hospitalInfo);
        }

        // GET: HospitalInfo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                var hospital = await _context.HospitalInfo.FirstOrDefaultAsync();
                if (hospital != null)
                    return RedirectToAction(nameof(Edit), new { id = hospital.HospitalId });

                return BadRequest();
            }

            var hospitalInfo = await _context.HospitalInfo.FindAsync(id);
            if (hospitalInfo == null)
                return NotFound();

            return View(hospitalInfo);
        }

        // POST: HospitalInfo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HospitalInfo hospitalInfo)
        {
            if (id != hospitalInfo.HospitalId)
            {
                return BadRequest();
            }

            // Remove validation for navigation properties and system-managed fields
            ModelState.Remove("InsurancePartnerships");
            ModelState.Remove("CreatedDate");
            ModelState.Remove("ModifiedDate");

            // Remove validation for optional fields not in the form
            ModelState.Remove("BankName");
            ModelState.Remove("TaxNumber");
            ModelState.Remove("BranchCode");
            ModelState.Remove("AccountNumber");
            ModelState.Remove("AccreditationBody");
            ModelState.Remove("HealthFacilityCode");
            ModelState.Remove("RegistrationNumber");
            ModelState.Remove("AccreditationDate");

            // Set default values for required fields if empty
            if (string.IsNullOrWhiteSpace(hospitalInfo.OperatingHours))
            {
                hospitalInfo.OperatingHours = "24/7";
            }

            // Set empty strings for fields that might be required in DB but not in form
            if (string.IsNullOrWhiteSpace(hospitalInfo.BankName))
                hospitalInfo.BankName = string.Empty;
            if (string.IsNullOrWhiteSpace(hospitalInfo.TaxNumber))
                hospitalInfo.TaxNumber = string.Empty;
            if (string.IsNullOrWhiteSpace(hospitalInfo.BranchCode))
                hospitalInfo.BranchCode = string.Empty;
            if (string.IsNullOrWhiteSpace(hospitalInfo.AccountNumber))
                hospitalInfo.AccountNumber = string.Empty;
            if (string.IsNullOrWhiteSpace(hospitalInfo.AccreditationBody))
                hospitalInfo.AccreditationBody = string.Empty;
            if (string.IsNullOrWhiteSpace(hospitalInfo.HealthFacilityCode))
                hospitalInfo.HealthFacilityCode = string.Empty;
            if (string.IsNullOrWhiteSpace(hospitalInfo.RegistrationNumber))
                hospitalInfo.RegistrationNumber = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the existing entity to preserve CreatedDate
                    var existingHospital = await _context.HospitalInfo.AsNoTracking()
                        .FirstOrDefaultAsync(h => h.HospitalId == id);

                    if (existingHospital == null)
                        return NotFound();

                    hospitalInfo.CreatedDate = existingHospital.CreatedDate;
                    hospitalInfo.ModifiedDate = DateTime.Now;

                    _context.Update(hospitalInfo);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Hospital information updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.HospitalInfo.AnyAsync(h => h.HospitalId == hospitalInfo.HospitalId))
                        return NotFound();

                    throw;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error updating hospital: {ex.Message}");
                    TempData["ErrorMessage"] = $"Error updating hospital: {ex.Message}";
                }
            }

            // Debug: Log validation errors
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                System.Diagnostics.Debug.WriteLine($"Validation Error: {error.ErrorMessage}");
            }

            return View(hospitalInfo);
        }

        // GET: HospitalInfo/ManageInsurance/5
        public async Task<IActionResult> ManageInsurance(int? id)
        {
            if (id == null)
                return BadRequest();

            var hospitalInfo = await _context.HospitalInfo
                .Include(h => h.InsurancePartnerships)
                .FirstOrDefaultAsync(h => h.HospitalId == id);

            if (hospitalInfo == null)
                return NotFound();

            ViewBag.HospitalId = id;
            return View(hospitalInfo.InsurancePartnerships.ToList());
        }

        // GET: HospitalInfo/CreateInsurance/5
        public IActionResult CreateInsurance(int hospitalId)
        {
            ViewBag.HospitalId = hospitalId;
            return View();
        }

        // POST: HospitalInfo/CreateInsurance
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInsurance(InsurancePartnership partnership)
        {
            ModelState.Remove("Hospital");

            if (ModelState.IsValid)
            {
                _context.InsurancePartnerships.Add(partnership);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Insurance partnership added successfully.";
                return RedirectToAction(nameof(ManageInsurance), new { id = partnership.HospitalId });
            }

            ViewBag.HospitalId = partnership.HospitalId;
            return View(partnership);
        }

        // GET: HospitalInfo/EditInsurance/5
        public async Task<IActionResult> EditInsurance(int? id)
        {
            if (id == null)
                return BadRequest();

            var partnership = await _context.InsurancePartnerships.FindAsync(id);
            if (partnership == null)
                return NotFound();

            return View(partnership);
        }

        // POST: HospitalInfo/EditInsurance/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInsurance(InsurancePartnership partnership)
        {
            ModelState.Remove("Hospital");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(partnership);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Insurance partnership updated successfully.";
                    return RedirectToAction(nameof(ManageInsurance), new { id = partnership.HospitalId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.InsurancePartnerships.AnyAsync(p => p.PartnershipId == partnership.PartnershipId))
                        return NotFound();

                    throw;
                }
            }

            return View(partnership);
        }

        // POST: HospitalInfo/DeleteInsurance/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInsurance(int id, int hospitalId)
        {
            var partnership = await _context.InsurancePartnerships.FindAsync(id);
            if (partnership != null)
            {
                _context.InsurancePartnerships.Remove(partnership);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Insurance partnership deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to delete: Insurance partnership not found.";
            }

            return RedirectToAction(nameof(ManageInsurance), new { id = hospitalId });
        }
    }
}