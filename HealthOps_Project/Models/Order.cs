namespace HealthOps_Project.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string? OrderedById { get; set; }
        public ICollection<OrderItem>? Items { get; set; }
    }
}
