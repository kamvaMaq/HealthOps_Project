namespace HealthOps_Project.Models
{
    public class Admission
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        public int? WardId { get; set; }
        public Ward? Ward { get; set; }
        public int? RoomId { get; set; }
        public Room? Room { get; set; }
        public int? BedId { get; set; }
        public Bed? Bed { get; set; }
        public DateTime AdmissionDate { get; set; } = DateTime.UtcNow;
        public DateTime? DischargeDate { get; set; }
        public string? Status { get; set; }
    }
}
