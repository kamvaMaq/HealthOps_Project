using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthOps_Project.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "South African ID is required")]
        [RegularExpression(@"^\d{13}$", ErrorMessage = "South African ID must be exactly 13 digits.")]
        public string SouthAfricanID { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(100)]
        public string Address { get; set; } = string.Empty;

        //public int? CurrentWardId { get; set; }

        //// Navigation property (optional but recommended)
        //public Ward CurrentWard { get; set; }


        [Required(ErrorMessage = "Gender is required")]
        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone Number is required")]
        [Phone]
        [StringLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [StringLength(20)]
        public string Email { get; set; } = string.Empty;



        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        public string? RoomNumber { get; set; }

        public bool IsActive { get; set; }


        public DateTime? AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }


        [Required(ErrorMessage = "Emergency Contact is required")]
        [StringLength(100)]
        public string EmergencyContact { get; set; } = string.Empty;

        [Required(ErrorMessage = "Medical History is required")]
        [StringLength(200)]
        public string MedicalHistory { get; set; } = string.Empty;


        [Required(ErrorMessage = "Age is required")]
        [StringLength(3)]
        public string Age { get; set; }

        [Required(ErrorMessage = "Allergies information is required")]
        [StringLength(200)]
        public string Allergies { get; set; } = string.Empty;

        //public bool isActive { get; set; } = true;

        // Foreign keys
        public int? WardId { get; set; }
        public int? BedId { get; set; }
        public string? DoctorUserId { get; set; }

        // Navigation properties
        public virtual Ward? Ward { get; set; }
        public virtual Bed? Bed { get; set; }
        public virtual ApplicationUser? DoctorUser { get; set; }


        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Diagnosis> Diagnoses { get; set; } = new List<Diagnosis>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public ICollection<PatientMovement> PatientMovements { get; set; } = new List<PatientMovement>();
        public ICollection<Discharge> Discharges { get; set; } = new List<Discharge>();
        public ICollection<VitalSign> VitalSigns { get; set; } = new List<VitalSign>();
        public ICollection<Symptom> Symptoms { get; set; } = new List<Symptom>();
        public ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
        [NotMapped]
        public ICollection<Visitation> Visitations { get; set; } = new List<Visitation>();
        public virtual ICollection<MedicationAdministration> MedicationAdministrations { get; set; } = new List<MedicationAdministration>();
        public virtual ICollection<Visitation> Visits { get; set; } = new List<Visitation>();


    }
}
