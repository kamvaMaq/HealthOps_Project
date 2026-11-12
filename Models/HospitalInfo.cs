using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class HospitalInfo
    {
        [Key]
        public int HospitalId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Hospital Name")]
        public string HospitalName { get; set; }

        [Required]
        [StringLength(500)]
        public string Address { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [StringLength(100)]
        public string Province { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        [Display(Name = "Main Phone")]
        public string MainPhone { get; set; }

        [Phone]
        [StringLength(20)]
        [Display(Name = "Emergency Phone")]
        public string EmergencyPhone { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(100)]
        public string Website { get; set; }

        [StringLength(50)]
        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; }

        [StringLength(50)]
        [Display(Name = "Tax Number")]
        public string TaxNumber { get; set; }

        [StringLength(50)]
        [Display(Name = "Health Facility Code")]
        public string HealthFacilityCode { get; set; }

        [StringLength(200)]
        [Display(Name = "Accreditation Body")]
        public string AccreditationBody { get; set; }

        [Display(Name = "Accreditation Date")]
        [DataType(DataType.Date)]
        public DateTime? AccreditationDate { get; set; }

        [Display(Name = "Total Beds")]
        public int TotalBeds { get; set; }

        [Display(Name = "ICU Beds")]
        public int ICUBeds { get; set; }

        [Display(Name = "Emergency Beds")]
        public int EmergencyBeds { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Operating Hours")]
        public string OperatingHours { get; set; }

        [Display(Name = "24/7 Operation")]
        public bool Is24Hour { get; set; }

        [StringLength(500)]
        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        [StringLength(100)]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [StringLength(50)]
        [Display(Name = "Branch Code")]
        public string BranchCode { get; set; }

        [StringLength(200)]
        [Display(Name = "Logo URL")]
        public string LogoUrl { get; set; }

        [StringLength(2000)]
        [Display(Name = "Mission Statement")]
        public string MissionStatement { get; set; }

        [StringLength(2000)]
        [Display(Name = "Policies")]
        public string Policies { get; set; }

        [Required]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        public virtual ICollection<InsurancePartnership> InsurancePartnerships { get; set; }
    }

    public class InsurancePartnership
    {
        [Key]
        public int PartnershipId { get; set; }

        [Required]
        public int HospitalId { get; set; }

        [ForeignKey("HospitalId")]
        public virtual HospitalInfo Hospital { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Insurance Provider")]
        public string InsuranceProvider { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Scheme Name")]
        public string SchemeName { get; set; }

        [StringLength(50)]
        [Display(Name = "Provider Code")]
        public string ProviderCode { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Contract Start Date")]
        [DataType(DataType.Date)]
        public DateTime? ContractStartDate { get; set; }

        [Display(Name = "Contract End Date")]
        [DataType(DataType.Date)]
        public DateTime? ContractEndDate { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }
    }
}

