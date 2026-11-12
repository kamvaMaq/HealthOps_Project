using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthOps_Project.Models
{
    public class MedicationAdministration
    {
        [Key]
        public int MedicationAdministrationId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int PrescriptionId { get; set; }

        [Required]
        [StringLength(200)]
        public string MedicationName { get; set; } = string.Empty;

        [Required]
        [Range(1, 6)]
        public int ScheduleLevel { get; set; }

        [Required]
        [StringLength(100)]
        public string Dosage { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public AdministrationRoute Route { get; set; }

        [Required]
        public DateTime AdministeredAt { get; set; } = DateTime.Now;

        [Required]
        [StringLength(100)]
        public string AdministeredBy { get; set; } = string.Empty;

        public string? Notes { get; set; }

        // Navigation properties
        public virtual Patient Patient { get; set; } = null!;

        [ForeignKey("PrescriptionId")]
        public virtual Prescription Prescription { get; set; } = null!;
        public virtual Medication? Medication { get; set; }
    }

    public enum AdministrationRoute
    {
        Oral,
        Intravenous,
        Intramuscular,
        Topical,
        Subcutaneous,
        Inhalation
    }
}


