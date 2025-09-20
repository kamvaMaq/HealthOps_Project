using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthOps_Project.Models
{
    public class Visitation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Patient")]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        public Patient? Patient { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Visitor Name")]
        public string VisitorName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Visit Date")]
        public DateTime VisitDate { get; set; }

        [StringLength(200)]
        public string? Purpose { get; set; }
    }
}
