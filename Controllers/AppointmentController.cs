using HealthOps_Project.Data;
using HealthOps_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        public IActionResult Appointments()
        {
            // Pass doctors to view
            var doctors = _userManager.GetUsersInRoleAsync("Doctor").Result;
            ViewBag.Doctors = doctors;
            return View();
        }


        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments.Where(a => a.isActive == true)
                .Select(a => new Appointment
                {
                    AppointmentId = a.AppointmentId,
                    PatientName = a.PatientName,
                    AppointmentTime = a.AppointmentTime,
                    PatientIdNumber = a.PatientIdNumber,
                    AppointmentDate = a.AppointmentDate,
                    Notes = a.Notes

                })
                .ToListAsync();

            return View(appointments);
        }
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                // Return all appointments or empty list if query is empty
                var allAppointments = await _context.Appointments.ToListAsync();
                return View("Index", allAppointments);
            }

            var filteredAppointments = await _context.Appointments
                                  .Where(p => p.PatientIdNumber.Contains(query))
                                  .ToListAsync();

            return View("Index", filteredAppointments);
        }


        // GET: Appointment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(m => m.AppointmentId == id);

            if (appointment == null) return NotFound();

            return View(appointment);
        }
        

        // POST: Appointment/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.isActive = false; // soft delete
                _context.Update(appointment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<JsonResult> GetBookedTimes(string doctorId, DateTime date)
        {
            var times = await _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == date.Date && a.isActive)
                .Select(a => a.AppointmentTime.ToString())
                .ToListAsync();

            return Json(times);
        }

        
        // GET: Create Appointment
        public async Task<IActionResult> Create()
        {
            ViewBag.Doctors = await _userManager.GetUsersInRoleAsync("Doctor");
            return View();
        }
        //qgoa ogrp etqg unkc"

        // POST: Create Appointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment model)
        {
            ViewBag.Doctors = await _userManager.GetUsersInRoleAsync("Doctor");

            if (!ModelState.IsValid)
                return View(model);

            bool slotTaken = await _context.Appointments.AnyAsync(a =>
                a.AppointmentDate.Date == model.AppointmentDate.Date &&
                a.AppointmentTime == model.AppointmentTime &&
                a.DoctorId == model.DoctorId &&
                a.isActive
            );

            if (slotTaken)
            {
                ModelState.AddModelError("", "This time slot is already booked for the selected doctor.");
                return View(model);
            }

            _context.Appointments.Add(model);
            await _context.SaveChangesAsync();

            var savedAppointment = await _context.Appointments
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.AppointmentId == model.AppointmentId);

            if (savedAppointment == null)
                return NotFound();

            // Send confirmation email
            if (!string.IsNullOrWhiteSpace(savedAppointment.PatientEmail))
            {
                try
                {
                    await SendConfirmationEmailHtml(savedAppointment);
                    TempData["EmailStatus"] = "✅ Confirmation email sent!";
                }
                catch (Exception ex)
                {
                    TempData["EmailStatus"] = $"❌ Failed to send email: {ex.Message}";
                }
            }
            else
            {
                TempData["EmailStatus"] = "⚠️ No patient email provided.";
            }

            return RedirectToAction("Confirmation", new { id = savedAppointment.AppointmentId });
        }

        // GET: Confirmation
        public async Task<IActionResult> Confirmation(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null) return NotFound();

            ViewBag.EmailStatus = TempData["EmailStatus"] as string;
            return View(appointment);
        }


        private async Task SendConfirmationEmailHtml(Appointment appointment)
        {
            if (string.IsNullOrWhiteSpace(appointment.PatientEmail))
                throw new Exception("Patient email is empty.");

            // SMTP settings
            var smtpHost = "smtp.gmail.com";           // Your SMTP server
            var smtpPort = 587;                        // TLS port
            var smtpUser = "sphesihlesbani@gmail.com";     // Your email
            var smtpPass = "qgoa ogrp etqg unkc";        // App password (if 2FA enabled)

            // Logo URL (hosted in wwwroot/images or external)
            var logoUrl = "/images/icon.jpeg";

            // Format AppointmentTime safely
            string timeFormatted = appointment.AppointmentTime.ToString(@"hh\:mm");

            // Email HTML body
            var body = $@"
        <div style='font-family: Arial, sans-serif; color: #333; line-height:1.5;'>
            <div style='background-color: #2E86C1; color: white; padding: 20px; text-align:center; border-radius: 5px 5px 0 0;'>
                <img src='{logoUrl}' alt='HealthOps Logo' style='height:50px; margin-bottom:10px;' />
                <h2>📅 Appointment Confirmation</h2>
            </div>
            <div style='padding: 20px;'>
                <p>Dear <strong>{appointment.PatientName}</strong>,</p>
                <p>Thank you for scheduling your appointment. Here are the details:</p>
                <table style='width:100%; border-collapse: collapse; margin-top:10px;'>
                    <tr>
                        <td style='padding:8px; font-weight:bold;'>Patient:</td>
                        <td style='padding:8px;'>{appointment.PatientName}</td>
                    </tr>
                    <tr style='background-color: #f2f2f2;'>
                        <td style='padding:8px; font-weight:bold;'>Date:</td>
                        <td style='padding:8px;'>{appointment.AppointmentDate:yyyy-MM-dd}</td>
                    </tr>
                    <tr>
                        <td style='padding:8px; font-weight:bold;'>Time:</td>
                        <td style='padding:8px;'>{timeFormatted}</td>
                    </tr>
                    <tr style='background-color: #f2f2f2;'>
                        <td style='padding:8px; font-weight:bold;'>Doctor:</td>
                        <td style='padding:8px;'>{ appointment.Doctor?.FullName ?? "N/A"}</td>
                    </tr>
                    {(!string.IsNullOrEmpty(appointment.Notes)
                                ? $"<tr><td style='padding:8px; font-weight:bold;'>Notes:</td><td style='padding:8px;'>{appointment.Notes}</td></tr>"
                                : "")}
                </table>
                <p style='margin-top:15px;'>We look forward to seeing you!</p>
                <p style='color:#777; font-size: 0.9em;'>This is an automated message from HealthOps.</p>
            </div>
        </div>
    ";

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new System.Net.NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(smtpUser, "HealthOps"),
                Subject = "✅ Your Appointment Confirmation",
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(appointment.PatientEmail);

            await client.SendMailAsync(mail);  // Async send
        }

    }
}