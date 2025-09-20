using System.ComponentModel.DataAnnotations.Schema;

namespace HealthOps_Project.Models
{
    public class Prescription
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public string? DoctorId { get; set; }
        public string? Medication { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Dosage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
