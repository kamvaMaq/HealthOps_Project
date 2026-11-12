using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{

    public class Admission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(PatientId))]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        [Required]
        public int WardId { get; set; }
        public Ward? Ward { get; set; }

        [Required]
        [ForeignKey(nameof(BedId))]
        public int BedId { get; set; }
        public Bed? Bed { get; set; }

        [Required]
        public DateTime AdmissionDate { get; set; }

        [Required]
        public DateTime? DischargeDate { get; set; }

        // Add this for soft delete functionality
        public bool IsActive { get; set; } = true;

        [Required]
        [ForeignKey(nameof(Doctor))]
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
    }
}
