using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class Notification
    {

        [Key]
        public int Id { get; set; }
        public string? Message { get; set; }
        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }
    }
}
