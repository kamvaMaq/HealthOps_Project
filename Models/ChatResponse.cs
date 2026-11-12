namespace HealthOps_Project.Models
{
    public class ChatResponse
    {
        public string Reply { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }

        public ChatResponse()
        {
            Timestamp = DateTime.Now;
        }
    }
}