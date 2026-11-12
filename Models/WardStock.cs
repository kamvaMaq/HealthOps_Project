using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace HealthOps_Project.Models
{
    public class WardStock
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ConsumableId { get; set; }

        [Required, StringLength(100)]
        public string WardName { get; set; }

        [Required]
        public int QuantityOnHand { get; set; }

        [ValidateNever]
        [ForeignKey("ConsumableId")]
        public virtual Consumable Consumable { get; set; }
    }
}
