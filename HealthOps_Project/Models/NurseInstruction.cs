namespace HealthOps_Project.Models
{
    public class NurseInstruction
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public string? Instruction { get; set; }
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public bool IsCompleted { get; set; }
    }
}
