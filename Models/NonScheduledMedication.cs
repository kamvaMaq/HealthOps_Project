using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class NonScheduledMedication
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
            [TodayDate(ErrorMessage = "Administered date must be today's date.")]
            public DateTime AdministeredDate { get; set; } = DateTime.Today;

            [Required]
            [StringLength(450, ErrorMessage = "Administered By cannot exceed 450 characters.")]
            public string AdministeredBy { get; set; }

            [Required]
            public NonScheduledMedicationStatus Status { get; set; } = NonScheduledMedicationStatus.Active;

            // Navigation properties
            public virtual Medication? Medication { get; set; }
            public virtual Patient? Patient { get; set; }
    }
 }
