using HealthOps_Project.Data;
using HealthOps_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthOps_Project.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Appointment
        public IActionResult Index()
        {
            // Pass doctors to view
            var doctors = _userManager.GetUsersInRoleAsync("Doctor").Result;
            ViewBag.Doctors = doctors;
            return View();
        }

        // GET month availability
        [HttpGet]
        public JsonResult GetMonthAvailability(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var appointments = _context.Appointments
                .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate)
                .ToList();

            var slots = new[]
            {
                new { Start = TimeSpan.Parse("06:45"), End = TimeSpan.Parse("08:00") },
                new { Start = TimeSpan.Parse("08:00"), End = TimeSpan.Parse("09:15") },
                new { Start = TimeSpan.Parse("09:15"), End = TimeSpan.Parse("11:00") },
                new { Start = TimeSpan.Parse("11:00"), End = TimeSpan.Parse("12:15") },
                new { Start = TimeSpan.Parse("13:00"), End = TimeSpan.Parse("14:15") },
                new { Start = TimeSpan.Parse("14:15"), End = TimeSpan.Parse("15:00") },
                new { Start = TimeSpan.Parse("15:00"), End = TimeSpan.Parse("16:15") },
                new { Start = TimeSpan.Parse("16:15"), End = TimeSpan.Parse("17:30") }
            };

            var result = new List<object>();
            var currentDate = startDate;

            while (currentDate <= endDate)
            {
                if (currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    result.Add(new
                    {
                        date = currentDate.ToString("yyyy-M-d"),
                        availableSlots = 0,
                        status = "weekend"
                    });
                }
                else
                {
                    int availableSlots = 0;
                    foreach (var slot in slots)
                    {
                        bool isBooked = appointments.Any(a =>
                            a.AppointmentDate.Date == currentDate.Date &&
                            a.AppointmentTime >= slot.Start &&
                            a.AppointmentTime < slot.End);

                        if (!isBooked) availableSlots++;
                    }

                    result.Add(new
                    {
                        date = currentDate.ToString("yyyy-M-d"),
                        availableSlots = availableSlots,
                        status = availableSlots == 0 ? "booked" : (availableSlots <= 2 ? "almost-full" : "free")
                    });
                }

                currentDate = currentDate.AddDays(1);
            }

            return Json(result);
        }

        // GET available slots for a specific date
        [HttpGet]
        public JsonResult GetSlotAvailability(DateTime date)
        {
            var slots = new[]
            {
                new { Start = TimeSpan.Parse("06:45"), End = TimeSpan.Parse("08:00") },
                new { Start = TimeSpan.Parse("08:00"), End = TimeSpan.Parse("09:15") },
                new { Start = TimeSpan.Parse("09:15"), End = TimeSpan.Parse("11:00") },
                new { Start = TimeSpan.Parse("11:00"), End = TimeSpan.Parse("12:15") },
                new { Start = TimeSpan.Parse("13:00"), End = TimeSpan.Parse("14:15") },
                new { Start = TimeSpan.Parse("14:15"), End = TimeSpan.Parse("15:00") },
                new { Start = TimeSpan.Parse("15:00"), End = TimeSpan.Parse("16:15") },
                new { Start = TimeSpan.Parse("16:15"), End = TimeSpan.Parse("17:30") }
            };

            var slotStatus = slots.Select(slot =>
            {
                var isBooked = _context.Appointments.Any(a =>
                    a.AppointmentDate.Date == date.Date &&
                    a.AppointmentTime >= slot.Start &&
                    a.AppointmentTime < slot.End
                );

                return new
                {
                    start = slot.Start.ToString(@"hh\:mm"),
                    end = slot.End.ToString(@"hh\:mm"),
                    available = !isBooked
                };
            });

            return Json(slotStatus);
        }

        // GET all doctors
        [HttpGet]
        public async Task<JsonResult> GetDoctors()
        {
            var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
            var result = doctors.Select(d => new
            {
                id = d.Id,
                name = d.FullName
            }).ToList();

            return Json(result);
        }

        // POST book appointment
        [HttpPost]
        public async Task<JsonResult> BookAppointment([FromBody] Appointment model)
        {
            if (model == null)
                return Json(new { success = false, message = "Invalid appointment data." });

            if (string.IsNullOrEmpty(model.PatientName))
                return Json(new { success = false, message = "Patient name is required." });

            if (string.IsNullOrEmpty(model.DoctorId))
                return Json(new { success = false, message = "Doctor selection is required." });

            bool slotTaken = _context.Appointments.Any(a =>
                a.AppointmentDate.Date == model.AppointmentDate.Date &&
                a.AppointmentTime == model.AppointmentTime &&
                a.DoctorId == model.DoctorId);

            if (slotTaken)
                return Json(new { success = false, message = "Selected time slot is already booked." });

            _context.Appointments.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Appointment booked successfully!" });
        }
    }
}
