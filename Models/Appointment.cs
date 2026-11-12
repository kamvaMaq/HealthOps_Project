using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthOps_Project.Models
{
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppointmentId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; } 
        [Required]
        public TimeSpan AppointmentTime { get; set; }

        // Patient info
        [Required, StringLength(200)]
        public string PatientName { get; set; }

        [StringLength(20)]
        public string PatientIdNumber { get; set; }

        [EmailAddress, StringLength(100)]
        public string PatientEmail { get; set; }

        [Phone, StringLength(15)]
        public string PatientPhone { get; set; }

        // Link to doctor (ApplicationUser)
        [Required, ForeignKey(nameof(Doctor))]
        public string? DoctorId { get; set; }

        public ApplicationUser? Doctor { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public bool isActive { get; set; } = true;
    }
}





