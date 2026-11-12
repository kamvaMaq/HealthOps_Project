using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class ScheduledMedication
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MedicationId { get; set; }

        [Required]
        public int PatientId { get; set; } // Link to patient

        [Required]
        [StringLength(100, ErrorMessage = "Dosage cannot exceed 100 characters.")]
        public string Dosage { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [FutureOrTodayDate(ErrorMessage = "Administered date must be today or a future date.")]
        public DateTime AdministeredDate { get; set; }

        [StringLength(100, ErrorMessage = "Administered By cannot exceed 100 characters.")]
        public string? AdministeredBy { get; set; }

        // Navigation properties
        public virtual Medication? Medication { get; set; }
        public virtual Patient? Patient { get; set; }

        [Required]
        public ScheduledMedicationStatus ScheduledMedicationStatus { get; set; } = ScheduledMedicationStatus.Active;
    }
}
