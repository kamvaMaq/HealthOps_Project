namespace HealthOps_Project.Models
{
    public class Ward
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<Room>? Rooms { get; set; }
    }
}
