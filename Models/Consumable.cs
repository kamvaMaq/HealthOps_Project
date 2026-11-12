using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class Consumable
    {
        public int ConsumableId { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [StringLength(50)]
        public string Unit { get; set; }
    }
}
