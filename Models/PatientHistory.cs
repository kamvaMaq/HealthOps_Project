using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class PatientHistory
    {

        [Key]
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
