using System.ComponentModel.DataAnnotations.Schema;

namespace HealthOps_Project.Models
{
    public class Script
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}
