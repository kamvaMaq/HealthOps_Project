using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class DoctorInstruction
    {
        [Key]
        public int DoctorInstructionId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        [StringLength(100)]
        public string DoctorName { get; set; } = string.Empty;

        [Required]
        public DateTime VisitDate { get; set; }

        [Required]
        [StringLength(2000)]
        public string Instructions { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsCompleted { get; set; } = false;

        [StringLength(20)]
        public string Priority { get; set; } = "Normal";

        // Navigation property
        public virtual Patient Patient { get; set; } = null!;
    }
}
