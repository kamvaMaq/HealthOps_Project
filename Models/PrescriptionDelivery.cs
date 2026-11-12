using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace HealthOps_Project.Models
{
    public class PrescriptionDelivery
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PrescriptionId { get; set; }

        [Required, StringLength(50)]
        public string Status { get; set; } = "Pending";

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeliveredAt { get; set; }

        [ValidateNever]
        [ForeignKey("PrescriptionId")]
        public virtual Prescription Prescription { get; set; }
    }
}
