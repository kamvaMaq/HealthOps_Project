using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty; 

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;  

        [Required]
        [StringLength(100)]
        public string Specialty { get; set; } = string.Empty;  

        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        //  Soft delete support
        public bool IsActive { get; set; } = true;

        // Navigation Properties - Initialize collections
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public ICollection<Diagnosis> Diagnoses { get; set; } = new List<Diagnosis>();
        //public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Visitation> Visitations { get; set; } = new List<Visitation>();
    }
}