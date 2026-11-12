using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class MedicationDelivery
    {
        [Key]
        public int DeliveryId { get; set; } // Primary key

        [Required]
        public int MedicationId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int DeliveredQuantity { get; set; }

        [Required]
        [Display(Name = "Delivery Date")]
        public DateTime DeliveredAt { get; set; } = DateTime.Now;

        public int ScriptId { get; set; }
        public int PrescriptionId { get; set; }
        public int OrderDeliveryItemId { get; set; }

        public string? MedicationName { get; set; }
        public int? ExpectedQuantity { get; set; }
        public int? ReceivedQuantity { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? ReceivedBy { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public bool IsVerified { get; set; } = false;
        public string? Notes { get; set; }

        // Navigation properties
        public virtual Medication Medication { get; set; } = null!;
        public virtual Script Script { get; set; } = null!;
        public virtual Prescription Prescription { get; set; } = null!;
       
    }
}
