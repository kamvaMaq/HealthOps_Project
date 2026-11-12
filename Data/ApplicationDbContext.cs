using HealthOps_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthOps_Project.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // --- Patient entities ---
        public DbSet<Patient> Patients { get; set; } = null!;
        public DbSet<PatientMovement> PatientMovements { get; set; } = null!;
        public DbSet<PatientHistory> PatientHistories { get; set; } = null!;
        public DbSet<Admission> Admissions { get; set; } = null!;
        public DbSet<Discharge> Discharges { get; set; } = null!;
        public DbSet<Visitation> Visitations { get; set; } = null!;

        // --- Medical entities ---
        public DbSet<Doctor> Doctors { get; set; } = null!;
        public DbSet<Symptom> Symptoms { get; set; } = null!;
        public DbSet<Diagnosis> Diagnoses { get; set; } = null!;
        public DbSet<Treatment> Treatments { get; set; } = null!;
        public DbSet<VitalSign> VitalSigns { get; set; } = null!;
        public DbSet<NurseInstruction> NurseInstructions { get; set; } = null!;
        public DbSet<DoctorInstruction> DoctorInstructions { get; set; } = null!;

        // --- Appointment & facility ---
        public DbSet<Appointment> Appointments { get; set; } = null!;
        public DbSet<Ward> Wards { get; set; } = null!;
        public DbSet<Room> Rooms { get; set; } = null!;
        public DbSet<Bed> Beds { get; set; } = null!;

        // --- Medication & prescription ---
        public DbSet<Prescription> Prescriptions { get; set; } = null!;
        public DbSet<Script> Scripts { get; set; } = null!;
        public DbSet<Medication> Medications { get; set; } = null!;
        public DbSet<MedicationAdministration> MedicationAdministrations { get; set; } = null!;
        public DbSet<ScheduledMedication> ScheduledMedications { get; set; } = null!;
        public DbSet<NonScheduledMedication> NonScheduledMedications { get; set; } = null!;


        // --- Inventory & orders ---
        
        public DbSet<Consumable> Consumables { get; set; } = null!;
        
        public DbSet<ConsumableRequest> ConsumableRequests { get; set; } = null!;
        public DbSet<PrescriptionDelivery> PrescriptionDeliveries { get; set; }
        public DbSet<WardStock> WardStocks { get; set; }
        
        public DbSet<Stocktake> Stocktakes { get; set; }

        // --- Delivery ---
        public DbSet<MedicationDelivery> MedicationDeliveries { get; set; } = null!;
       
        

        // --- Communication ---
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<ContactMessage> ContactMessages { get; set; } = null!;
        public DbSet<EmailMessage> EmailMessages { get; set; } = null!;
        public DbSet<ChatMessage> ChatMessages { get; set; } = null!;

        // --- Insurance ---
        public DbSet<InsuranceProvider> InsuranceProviders { get; set; } = null!;

        // Employee Management
        public DbSet<Employee> Employees { get; set; }
        //public DbSet<EmployeeRole> EmployeeRoles { get; set; }
        public DbSet<Department> Departments { get; set; }
       

        // Allergy Management
        public DbSet<Allergy> Allergies { get; set; }
        public DbSet<PatientAllergy> PatientAllergies { get; set; }

        // Medical Condition Management
        public DbSet<MedicalCondition> MedicalConditions { get; set; }
        public DbSet<PatientCondition> PatientConditions { get; set; }

        // Hospital Info Management
        public DbSet<HospitalInfo> HospitalInfo { get; set; }
        public DbSet<InsurancePartnership> InsurancePartnerships { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(u => u.LastName).HasMaxLength(100).IsRequired();
                entity.Property(u => u.StaffId).HasMaxLength(50).IsRequired();
                entity.Property(u => u.IsActive).HasDefaultValue(true);

                // FullName is computed, not stored
                entity.Ignore(u => u.FullName);
            });

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<MedicationDelivery>(entity =>
            {
                entity.HasKey(md => md.DeliveryId);

                // Prescription FK: Cascade
                entity.HasOne(md => md.Prescription)
                      .WithMany(p => p.MedicationDeliveries)
                      .HasForeignKey(md => md.PrescriptionId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Script FK: Restrict to avoid multiple cascade paths
                entity.HasOne(md => md.Script)
                      .WithMany(s => s.MedicationDeliveries)
                      .HasForeignKey(md => md.ScriptId)
                      .OnDelete(DeleteBehavior.Restrict);

                //// Medication FK: Restrict
                //entity.HasOne(md => md.Medication)
                //      .WithMany(m => m.MedicationDeliveries)
                //      .HasForeignKey(md => md.MedicationId)
                //      .OnDelete(DeleteBehavior.Restrict);


            });


            modelBuilder.Entity<MedicationAdministration>(entity =>
            {
                entity.HasOne(ma => ma.Prescription)
                      .WithMany(p => p.MedicationAdministrations)
                      .HasForeignKey(ma => ma.PrescriptionId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ma => ma.Patient)
                      .WithMany(p => p.MedicationAdministrations)
                      .HasForeignKey(ma => ma.PatientId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            // ===========================
            // Seed Roles Only
            // ===========================
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "role-doctor", Name = "Doctor", NormalizedName = "DOCTOR" },
                new IdentityRole { Id = "role-stock", Name = "StockManager", NormalizedName = "STOCKMANAGER" },
                new IdentityRole { Id = "role-script", Name = "ScriptManager", NormalizedName = "SCRIPTMANAGER" },
                new IdentityRole { Id = "role-admin", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "role-nurse", Name = "Nurse", NormalizedName = "NURSE" },
                new IdentityRole { Id = "role-nursing-sister", Name = "NursingSister", NormalizedName = "NURSINGSISTER" }
            );


            // ===========================
            // Seed Patients (with shorter emails)
            // ===========================
            modelBuilder.Entity<Patient>().HasData(
                new Patient
                {
                    PatientId = 1,
                    SouthAfricanID = "8501015123089",
                    FirstName = "James",
                    LastName = "Wilson",
                    DOB = new DateTime(1985, 1, 1),
                    Gender = "Male",
                    PhoneNumber = "0711234567",
                    Email = "james@email.com",
                    Address = "123 Main Street, Johannesburg",
                    EmergencyContact = "0729876543",
                    MedicalHistory = "Hypertension, Type 2 Diabetes",
                    Age = "38",
                    Allergies = "Penicillin, Shellfish",
                    IsActive = true,
                    AdmissionDate = DateTime.UtcNow.AddDays(-5),
                    RoomNumber = "101A"
                },
                new Patient
                {
                    PatientId = 2,
                    SouthAfricanID = "9005154123085",
                    FirstName = "Sarah",
                    LastName = "Johnson",
                    DOB = new DateTime(1990, 5, 15),
                    Gender = "Female",
                    PhoneNumber = "0824567890",
                    Email = "sarah@email.com",
                    Address = "456 Oak Avenue, Cape Town",
                    EmergencyContact = "0831234567",
                    MedicalHistory = "Asthma, Migraines",
                    Age = "33",
                    Allergies = "Aspirin, Dust mites",
                    IsActive = true,
                    AdmissionDate = DateTime.UtcNow.AddDays(-2),
                    RoomNumber = "205B"
                },
                new Patient
                {
                    PatientId = 3,
                    SouthAfricanID = "8802206123087",
                    FirstName = "Robert",
                    LastName = "Brown",
                    DOB = new DateTime(1988, 2, 20),
                    Gender = "Male",
                    PhoneNumber = "0741122334",
                    Email = "robert@email.com",
                    Address = "789 Pine Road, Durban",
                    EmergencyContact = "0745566778",
                    MedicalHistory = "High cholesterol, Back pain",
                    Age = "35",
                    Allergies = "Ibuprofen, Latex",
                    IsActive = true,
                    AdmissionDate = DateTime.UtcNow.AddDays(-1),
                    RoomNumber = "312C"
                },
                new Patient
                {
                    PatientId = 4,
                    SouthAfricanID = "9208103123081",
                    FirstName = "Lisa",
                    LastName = "Davis",
                    DOB = new DateTime(1992, 8, 10),
                    Gender = "Female",
                    PhoneNumber = "0769988776",
                    Email = "lisa@email.com",
                    Address = "321 Beach Road, Port Elizabeth",
                    EmergencyContact = "0765544332",
                    MedicalHistory = "Anxiety, Insomnia",
                    Age = "31",
                    Allergies = "None known",
                    IsActive = true,
                    AdmissionDate = DateTime.UtcNow,
                    RoomNumber = "108D"
                },
                new Patient
                {
                    PatientId = 5,
                    SouthAfricanID = "8706301123083",
                    FirstName = "Michael",
                    LastName = "Taylor",
                    DOB = new DateTime(1987, 6, 30),
                    Gender = "Male",
                    PhoneNumber = "0736655443",
                    Email = "michael@email.com",
                    Address = "654 Mountain View, Pretoria",
                    EmergencyContact = "0732211334",
                    MedicalHistory = "GERD, Seasonal allergies",
                    Age = "36",
                    Allergies = "Pollen, Cat dander",
                    IsActive = false,
                    AdmissionDate = DateTime.UtcNow.AddDays(-10),
                    DischargeDate = DateTime.UtcNow.AddDays(-2),
                    RoomNumber = "209E"
                }
            );

            // ===========================
            // Seed Consumables
            // ===========================
            modelBuilder.Entity<Consumable>().HasData(
                new Consumable { ConsumableId = 1, Name = "Gloves", Unit = "Box", Description = "Latex gloves, medium size, 100 per box" },
                new Consumable { ConsumableId = 2, Name = "Syringes", Unit = "Pack", Description = "5ml disposable syringes, 50 per pack" },
                new Consumable { ConsumableId = 3, Name = "Linen Savers", Unit = "Roll", Description = "Disposable linen savers for beds, 10 per roll" },
                new Consumable { ConsumableId = 4, Name = "Bandages", Unit = "Pack", Description = "Sterile gauze bandages, 10 per pack" },
                new Consumable { ConsumableId = 5, Name = "Alcohol Swabs", Unit = "Box", Description = "Isopropyl alcohol swabs, 200 per box" },
                new Consumable { ConsumableId = 6, Name = "Gauze Pads", Unit = "Pack", Description = "Sterile gauze pads 10x10cm, 25 per pack" },
                new Consumable { ConsumableId = 7, Name = "Medical Tape", Unit = "Roll", Description = "Hypoallergenic medical tape, 10m per roll" }
            );

            // ===========================
            // Seed WardStock
            // ===========================
            modelBuilder.Entity<WardStock>().HasData(
                new WardStock { Id = 1, WardName = "Ward A", ConsumableId = 1, QuantityOnHand = 50 },
                new WardStock { Id = 2, WardName = "Ward B", ConsumableId = 2, QuantityOnHand = 100 },
                new WardStock { Id = 3, WardName = "Ward A", ConsumableId = 3, QuantityOnHand = 25 },
                new WardStock { Id = 4, WardName = "Ward C", ConsumableId = 4, QuantityOnHand = 75 },
                new WardStock { Id = 5, WardName = "Emergency", ConsumableId = 5, QuantityOnHand = 150 },
                new WardStock { Id = 6, WardName = "ICU", ConsumableId = 6, QuantityOnHand = 40 },
                new WardStock { Id = 7, WardName = "Ward B", ConsumableId = 7, QuantityOnHand = 30 }
            );

            // ===========================
            // Seed Prescriptions
            // ===========================
            modelBuilder.Entity<Prescription>().HasData(
                new Prescription
                {
                    PrescriptionId = 1,
                    PatientId = 1,
                    MedicationName = "Paracetamol 500mg",
                    Dosage = "500mg",
                    Frequency = "Every 8 hours",
                    PrescribingDoctor = "Dr John Doe",
                    StartDate = DateTime.UtcNow.AddDays(-5),
                    EndDate = DateTime.UtcNow.AddDays(5),
                    Instructions = "Take after meals with plenty of water",
                    Quantity = 30,
                    Status = "Active",
                    Priority = "Routine",
                    Notes = "Patient has mild fever",
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new Prescription
                {
                    PrescriptionId = 2,
                    PatientId = 2,
                    MedicationName = "Amoxicillin 250mg",
                    Dosage = "250mg",
                    Frequency = "Three times daily",
                    PrescribingDoctor = "Dr John Doe",
                    StartDate = DateTime.UtcNow.AddDays(-2),
                    EndDate = DateTime.UtcNow.AddDays(8),
                    Instructions = "Complete the full course even if feeling better",
                    Quantity = 21,
                    Status = "Active",
                    Priority = "Urgent",
                    Notes = "For bacterial infection",
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new Prescription
                {
                    PrescriptionId = 3,
                    PatientId = 3,
                    MedicationName = "Ibuprofen 400mg",
                    Dosage = "400mg",
                    Frequency = "As needed",
                    PrescribingDoctor = "Dr Sarah Johnson",
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(14),
                    Instructions = "Take for pain relief, maximum 3 times per day",
                    Quantity = 20,
                    Status = "Pending",
                    Priority = "Routine",
                    Notes = "For back pain management",
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new Prescription
                {
                    PrescriptionId = 4,
                    PatientId = 1,
                    MedicationName = "Ventolin Inhaler",
                    Dosage = "100mcg",
                    Frequency = "Twice daily",
                    PrescribingDoctor = "Dr John Doe",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(30),
                    Instructions = "Use one puff in morning and evening",
                    Quantity = 1,
                    Status = "Processed",
                    Priority = "High",
                    Notes = "Asthma maintenance medication",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Prescription
                {
                    PrescriptionId = 5,
                    PatientId = 2,
                    MedicationName = "Omeprazole 20mg",
                    Dosage = "20mg",
                    Frequency = "Once daily",
                    PrescribingDoctor = "Dr Sarah Johnson",
                    StartDate = DateTime.UtcNow.AddDays(-10),
                    EndDate = DateTime.UtcNow.AddDays(20),
                    Instructions = "Take before breakfast",
                    Quantity = 30,
                    Status = "Dispensed",
                    Priority = "Medium",
                    Notes = "For acid reflux",
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new Prescription
                {
                    PrescriptionId = 6,
                    PatientId = 4,
                    MedicationName = "Atorvastatin 40mg",
                    Dosage = "40mg",
                    Frequency = "Once daily",
                    PrescribingDoctor = "Dr John Doe",
                    StartDate = DateTime.UtcNow.AddDays(-3),
                    EndDate = DateTime.UtcNow.AddDays(27),
                    Instructions = "Take at bedtime",
                    Quantity = 30,
                    Status = "New",
                    Priority = "High",
                    Notes = "For cholesterol management",
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    UpdatedAt = DateTime.UtcNow.AddDays(-3)
                }
            );

            // ===========================
            // Seed PrescriptionDeliveries
            // ===========================
            modelBuilder.Entity<PrescriptionDelivery>().HasData(
                new PrescriptionDelivery
                {
                    Id = 1,
                    PrescriptionId = 1,
                    Status = "Pending",
                    RequestedAt = DateTime.UtcNow.AddHours(-2)
                },
                new PrescriptionDelivery
                {
                    Id = 2,
                    PrescriptionId = 2,
                    Status = "Delivered",
                    RequestedAt = DateTime.UtcNow.AddDays(-1),
                    DeliveredAt = DateTime.UtcNow.AddHours(-4)
                },
                new PrescriptionDelivery
                {
                    Id = 3,
                    PrescriptionId = 4,
                    Status = "Pending",
                    RequestedAt = DateTime.UtcNow.AddHours(-1)
                },
                new PrescriptionDelivery
                {
                    Id = 4,
                    PrescriptionId = 5,
                    Status = "Delivered",
                    RequestedAt = DateTime.UtcNow.AddDays(-2),
                    DeliveredAt = DateTime.UtcNow.AddDays(-1)
                },
                new PrescriptionDelivery
                {
                    Id = 5,
                    PrescriptionId = 6,
                    Status = "Cancelled",
                    RequestedAt = DateTime.UtcNow.AddHours(-6),
                    DeliveredAt = null
                }
            );

            // ===========================
            // Seed ConsumableRequests
            // ===========================
            modelBuilder.Entity<ConsumableRequest>().HasData(
                new ConsumableRequest
                {
                    Id = 1,
                    ConsumableId = 2,
                    WardName = "Ward B",
                    QuantityRequested = 30,
                    Status = "Pending",
                    RequestedAt = DateTime.UtcNow.AddHours(-2)
                },
                new ConsumableRequest
                {
                    Id = 2,
                    ConsumableId = 1,
                    WardName = "Emergency",
                    QuantityRequested = 10,
                    Status = "Approved",
                    RequestedAt = DateTime.UtcNow.AddDays(-1),
                    ReceivedAt = DateTime.UtcNow.AddHours(-4)
                },
                new ConsumableRequest
                {
                    Id = 3,
                    ConsumableId = 4,
                    WardName = "Ward A",
                    QuantityRequested = 15,
                    Status = "Received",
                    RequestedAt = DateTime.UtcNow.AddDays(-2),
                    ReceivedAt = DateTime.UtcNow.AddHours(-1)
                },
                new ConsumableRequest
                {
                    Id = 4,
                    ConsumableId = 5,
                    WardName = "ICU",
                    QuantityRequested = 5,
                    Status = "Pending",
                    RequestedAt = DateTime.UtcNow.AddHours(-3)
                }
            );

            // ===========================
            // Seed Stocktakes
            // ===========================
            modelBuilder.Entity<Stocktake>().HasData(
                new Stocktake
                {
                    Id = 1,
                    ConsumableId = 1,
                    WardName = "Ward A",
                    QuantityCounted = 50,
                    DateTaken = DateTime.UtcNow.AddDays(-1),
                    Notes = "Initial stocktake - all items accounted for",
                },
                new Stocktake
                {
                    Id = 2,
                    ConsumableId = 2,
                    WardName = "Ward B",
                    QuantityCounted = 95,
                    DateTaken = DateTime.UtcNow,
                    Notes = "Minor discrepancy noted - 5 units missing",
                },
                new Stocktake
                {
                    Id = 3,
                    ConsumableId = 5,
                    WardName = "Emergency",
                    QuantityCounted = 148,
                    DateTaken = DateTime.UtcNow.AddHours(-2),
                    Notes = "2 units damaged, removed from stock",
                }
            );
        }

           
         

    }


}


