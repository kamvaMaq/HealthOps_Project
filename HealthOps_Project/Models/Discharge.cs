namespace HealthOps_Project.Models
{
    public class Discharge
    {
        public int Id { get; set; }
        public int AdmissionId { get; set; }
        public Admission? Admission { get; set; }
        public DateTime DischargeDate { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }
    }
}
