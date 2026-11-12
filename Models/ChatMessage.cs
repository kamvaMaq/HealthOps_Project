using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class ChatMessage
    {

        [Key]
        public int Id { get; set; }
        public string? SenderId { get; set; }
        public string? ReceiverId { get; set; }
        public string? Message { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
