namespace HealthOps_Project.Models
{
    public class Diagnosis
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public int? SymptomId { get; set; }
        public Symptom? Symptom { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
