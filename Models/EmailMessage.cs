using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class EmailMessage
    {

        [Key]
        public int Id { get; set; }
        public string? ToEmail { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public DateTime? SentAt { get; set; }
    }
}
