using System.ComponentModel.DataAnnotations.Schema;

namespace HealthOps_Project.Models
{
    public class VitalSign
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        public decimal Temperature { get; set; }
        [Column(TypeName = "decimal(7,2)")]
        public decimal BloodPressure { get; set; }
        [Column(TypeName = "decimal(6,2)")]
        public decimal Pulse { get; set; }
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    }
}
