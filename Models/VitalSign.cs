using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthOps_Project.Models
{
    public class VitalSign
    {
        [Key]
        public int VitalId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public DateTime RecordedDate { get; set; } = DateTime.Now;

        [StringLength(20)]
        public string? BloodPressure { get; set; }

        [Range(30, 45)]
        [Column(TypeName = "decimal(5,2)")]  
        public decimal? Temperature { get; set; }

        [Range(50, 400)]
        public int? BloodSugar { get; set; }

        [Range(40, 200)]
        public int? HeartRate { get; set; }

        [Range(70, 100)]
        public int? OxygenSaturation { get; set; }

        [Range(20, 300)]
        [Column(TypeName = "decimal(8,2)")]  
        public decimal? Weight { get; set; }

        [Required]
        [StringLength(100)]
        public string RecordedBy { get; set; } = string.Empty;

        public string? Notes { get; set; }

        // Navigation property
        public virtual Patient Patient { get; set; } = null!;
    }
}

//public class VitalSign
//{
//    public int Id { get; set; }
//    public int PatientId { get; set; }
//    public Patient? Patient { get; set; }
//    [Column(TypeName = "decimal(5,2)")]
//    public decimal Temperature { get; set; }
//    [Column(TypeName = "decimal(7,2)")]
//    public decimal BloodPressure { get; set; }
//    [Column(TypeName = "decimal(6,2)")]
//    public decimal Pulse { get; set; }
//    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
//}

