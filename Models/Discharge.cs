using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class Discharge
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DischargeDate { get; set; }

        [StringLength(200)]
        public string? Reason { get; set; }

        [ForeignKey(nameof(Patient))]
        public int? PatientId { get; set; } // made nullable for flexibility

        [ForeignKey(nameof(Ward))]
        public int? WardId { get; set; } // made nullable for flexibility

        // Soft delete support
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public Patient? Patient { get; set; }
        public Ward? Ward { get; set; }
    }
}
