using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }



        [Required]
        [StringLength(100)]
        public string Specialty { get; set; }

        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        // ✅ Soft delete support
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public ICollection<Prescription> Prescriptions { get; set; }
        public ICollection<Diagnosis> Diagnoses { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Visitation> Visitations { get; set; }
    }
}
