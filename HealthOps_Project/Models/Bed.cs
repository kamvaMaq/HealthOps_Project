namespace HealthOps_Project.Models
{
    public class Bed
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public Room? Room { get; set; }
        public string Name { get; set; } = null!;
        public bool IsOccupied { get; set; }
    }
}
