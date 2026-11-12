using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthOps_Project.Models
{
    public class Script
    {

        [Key]
        public int ScriptId { get; set; }
        public int PatientId { get; set; }
        public string PrescribingDoctor { get; set; }
        public string MedicationName { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; } = "New"; // Default status
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ForwardedAt { get; set; }
        public string ForwardedBy { get; set; }
      

        // Navigation property
        public virtual Patient Patient { get; set; }

        public virtual ICollection<MedicationDelivery> MedicationDeliveries { get; set; } = new List<MedicationDelivery>();


    }


}
//public class Script
//{
//    public int Id { get; set; }
//    public string Name { get; set; } = null!;
//    public string? Description { get; set; }
//    [Column(TypeName = "decimal(18,2)")]
//    public decimal Price { get; set; }
//}