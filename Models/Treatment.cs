using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthOps_Project.Models
{

    public class Treatment
    {
        [Key]
        public int TreatmentId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public DateTime TreatmentDate { get; set; } = DateTime.Now;

        [Required]
        public TreatmentType TreatmentType { get; set; }

        [Required]
        [StringLength(1000)]
        public string Details { get; set; } = string.Empty;

        public string? Notes { get; set; }

        [Required]
        [StringLength(100)]
        public string PerformedBy { get; set; } = string.Empty;

        // Navigation property
        public virtual Patient Patient { get; set; } = null!;
    }

    public enum TreatmentType
    {
        IVDripChange,
        CatheterChange,
        WoundDressing,
        MedicationAdministration,
        Other
    }
    //public class Treatment
    //{
    //    public int Id { get; set; }
    //    public int PatientId { get; set; }
    //    public Patient? Patient { get; set; }
    //    public int? DiagnosisId { get; set; }
    //    public Diagnosis? Diagnosis { get; set; }
    //    [Column(TypeName = "decimal(18,2)")]
    //    public decimal Cost { get; set; }
    //    public string? Description { get; set; }
    //}
}
