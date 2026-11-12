using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class ChatRequest
    {
        [Required(ErrorMessage = "Message is required")]
        [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
        public string Message { get; set; }

        public string Context { get; set; } = "HealthOPS Healthcare System";
    }
}