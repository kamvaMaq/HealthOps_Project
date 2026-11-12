using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class Diagnosis
    {

        [Key]
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public int? SymptomId { get; set; }
        public Symptom? Symptom { get; set; }
        public string? Description { get; set; }

        public int DoctorId { get; set; }  // FK property

        public Doctor Doctor { get; set; } // Navigation property
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
