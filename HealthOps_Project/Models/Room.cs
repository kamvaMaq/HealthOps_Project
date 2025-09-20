namespace HealthOps_Project.Models
{
    public class Room
    {
        public int Id { get; set; }
        public int WardId { get; set; }
        public Ward? Ward { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<Bed>? Beds { get; set; }
    }
}
