using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthOps_Project.Models
{
    public class Prescription
    {
        [Key]
        public int PrescriptionId { get; set; }
        [Required]
        public int PatientId { get; set; }

        public Patient? Patient { get; set; }

        [Required(ErrorMessage = "Medication name is required")]
        public string MedicationName { get; set; }

        [Required(ErrorMessage = "Dosage is required")]
        public string Dosage { get; set; }

        [Required(ErrorMessage = "Frequency is required")]
        public string Frequency { get; set; }

        [Required(ErrorMessage = "Prescribing doctor name is required")]
        public string PrescribingDoctor { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Instructions { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = "Active";

        public string Priority { get; set; } = "Routine";

        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

     

        public virtual ICollection<MedicationAdministration> AdministrationRecords { get; set; }
            = new List<MedicationAdministration>();

        // Initialize collections to avoid null reference
        public virtual ICollection<MedicationDelivery> MedicationDeliveries { get; set; }
            = new List<MedicationDelivery>();

        public virtual ICollection<MedicationAdministration> MedicationAdministrations { get; set; }
            = new List<MedicationAdministration>();
        public virtual ICollection<PrescriptionDelivery> PrescriptionDeliveries { get; set; }
            = new List<PrescriptionDelivery>();
    }
}