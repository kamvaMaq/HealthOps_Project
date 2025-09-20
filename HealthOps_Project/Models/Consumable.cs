using System.ComponentModel.DataAnnotations.Schema;

namespace HealthOps_Project.Models
{
    public class Consumable
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}
