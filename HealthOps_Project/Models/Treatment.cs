using System.ComponentModel.DataAnnotations.Schema;

namespace HealthOps_Project.Models
{
    public class Treatment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public int? DiagnosisId { get; set; }
        public Diagnosis? Diagnosis { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }
        public string? Description { get; set; }
    }
}
