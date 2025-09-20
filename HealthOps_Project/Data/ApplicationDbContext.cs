using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HealthOps_Project.Models;

namespace HealthOps_Project.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientHistory> PatientHistories { get; set; }
        public DbSet<Admission> Admissions { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Discharge> Discharges { get; set; }
        public DbSet<Visitation> Visitations { get; set; }
        public DbSet<Symptom> Symptoms { get; set; }
        public DbSet<Diagnosis> Diagnoses { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<VitalSign> VitalSigns { get; set; }
        public DbSet<NurseInstruction> NurseInstructions { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Bed> Beds { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Script> Scripts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Consumable> Consumables { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<InsuranceProvider> InsuranceProviders { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<EmailMessage> EmailMessages { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Decimal precision configuration
            builder.Entity<Treatment>().Property(t => t.Cost).HasColumnType("decimal(18,2)");
            builder.Entity<Prescription>().Property(p => p.Dosage).HasColumnType("decimal(18,2)");
            builder.Entity<OrderItem>().Property(o => o.Quantity).HasColumnType("decimal(18,2)");
            builder.Entity<OrderItem>().Property(o => o.UnitPrice).HasColumnType("decimal(18,2)");
            builder.Entity<Consumable>().Property(c => c.Quantity).HasColumnType("decimal(18,2)");
            builder.Entity<Consumable>().Property(c => c.Price).HasColumnType("decimal(18,2)");
            builder.Entity<VitalSign>().Property(v => v.Temperature).HasColumnType("decimal(5,2)");
            builder.Entity<VitalSign>().Property(v => v.BloodPressure).HasColumnType("decimal(7,2)");
        }
    }
}
