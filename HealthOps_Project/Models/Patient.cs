using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthOps_Project.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }    

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Range(0, 150)]
        public int Age { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        public bool IsActive { get; set; }

        // Navigation Properties
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Diagnosis> Diagnoses { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; }
        public ICollection<PatientMovement> PatientMovements { get; set; }
        public ICollection<Discharge> Discharges { get; set; }
        public ICollection<VitalSign> VitalSigns { get; set; }
        public ICollection<Symptom> Symptoms { get; set; }
        public ICollection<Treatment> Treatments { get; set; }
        public ICollection<Visitation> Visitations { get; set; }
    }
}
