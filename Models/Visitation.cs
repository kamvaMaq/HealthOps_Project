using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthOps_Project.Models
{
    public class Visitation
    {
        [Key]
        public int VisitId { get; set; }

        [Required(ErrorMessage = "Patient is required")]
        [ForeignKey("Patient")]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }


        public bool isActive { get; set; } = true;
        // Navigation property
        public virtual Patient? Patient { get; set; }

        [Required(ErrorMessage = "Visitor name is required")]
        [StringLength(100)]
        [Display(Name = "Visitor Name")]
        public string VisitorName { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Purpose { get; set; }

        [Required(ErrorMessage = "Scheduled date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Scheduled Date")]
        public DateTime ScheduledDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Scheduled time is required")]
        [DataType(DataType.Time)]
        [Display(Name = "Scheduled Time")]
        public TimeSpan ScheduledTime { get; set; } = DateTime.Now.TimeOfDay;

        // This will be calculated from ScheduledDate + ScheduledTime
        public DateTime VisitDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        public string Status { get; set; } = "Scheduled";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;



    }
}