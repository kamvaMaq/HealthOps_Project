
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class MedicalCondition
    {
        [Key]
        public int ConditionId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Condition Name")]
        public string ConditionName { get; set; }

        [Required]
        [Display(Name = "Condition Category")]
        public ConditionCategory Category { get; set; }

        [StringLength(10)]
        [Display(Name = "ICD-10 Code")]
        public string ICD10Code { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        [StringLength(2000)]
        [Display(Name = "Treatment Guidelines")]
        public string TreatmentGuidelines { get; set; }

        [StringLength(1000)]
        [Display(Name = "Common Symptoms")]
        public string CommonSymptoms { get; set; }

        [Display(Name = "Requires Monitoring")]
        public bool RequiresMonitoring { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Required]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        public virtual ICollection<PatientCondition> PatientConditions { get; set; }
    }

    public enum ConditionCategory
    {
        Chronic,
        Acute,
        Genetic,
        Infectious,
        Autoimmune,
        Metabolic,
        Neurological,
        Cardiovascular,
        Respiratory,
        Other
    }

    public class PatientCondition
    {
        [Key]
        public int PatientConditionId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int ConditionId { get; set; }

        [ForeignKey("ConditionId")]
        public virtual MedicalCondition Condition { get; set; }

        [Required]
        [Display(Name = "Date Diagnosed")]
        [DataType(DataType.Date)]
        public DateTime DateDiagnosed { get; set; }

        [Required]
        [Display(Name = "Condition Status")]
        public ConditionStatus Status { get; set; }

        [StringLength(1000)]
        [Display(Name = "Treatment Plan")]
        public string TreatmentPlan { get; set; }

        [Display(Name = "Diagnosed By")]
        public int? DiagnosedByEmployeeId { get; set; }

        [Display(Name = "Resolution Date")]
        [DataType(DataType.Date)]
        public DateTime? ResolutionDate { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }

    public enum ConditionStatus
    {
        Active,
        Controlled,
        Uncontrolled,
        InRemission,
        Resolved,
        Critical
    }
}

