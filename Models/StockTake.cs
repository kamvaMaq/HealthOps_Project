using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace HealthOps_Project.Models
{
    public class Stocktake
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ConsumableId { get; set; }

        [Required, StringLength(100)]
        public string WardName { get; set; }

        [Required]
        public int QuantityCounted { get; set; }

        [Required]
        public DateTime DateTaken { get; set; } = DateTime.UtcNow;

        public string Notes { get; set; }

        [ValidateNever]
        public virtual Consumable Consumable { get; set; }
    }
}
