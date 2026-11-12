using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class Allergy
    {
        [Key]
        public int AllergyId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Allergy Name")]
        public string AllergyName { get; set; }

        [Required]
        [Display(Name = "Allergen Type")]
        public AllergenType AllergenType { get; set; }

        [Required]
        [Display(Name = "Severity Level")]
        public AllergySeverity SeverityLevel { get; set; }

        [StringLength(1000)]
        [Display(Name = "Common Reactions")]
        public string CommonReactions { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(500)]
        [Display(Name = "Treatment Protocol")]
        public string TreatmentProtocol { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Required]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        public virtual ICollection<PatientAllergy> PatientAllergies { get; set; }
    }

    public enum AllergenType
    {
        Medication,
        Food,
        Environmental,
        Latex,
        Chemical,
        Insect,
        Other
    }

    public enum AllergySeverity
    {
        Mild,
        Moderate,
        Severe,
        LifeThreatening
    }

    public class PatientAllergy
    {
        [Key]
        public int PatientAllergyId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int AllergyId { get; set; }

        [ForeignKey("AllergyId")]
        public virtual Allergy Allergy { get; set; }

        [Display(Name = "Date Identified")]
        [DataType(DataType.Date)]
        public DateTime DateIdentified { get; set; }

        [StringLength(1000)]
        [Display(Name = "Patient Specific Reaction")]
        public string PatientSpecificReaction { get; set; }

        [Display(Name = "Verified")]
        public bool IsVerified { get; set; }

        [Display(Name = "Verified By")]
        public int? VerifiedByEmployeeId { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }
    }
}

