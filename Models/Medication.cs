using HealthOps_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class Medication
    {
        [Key]
        public int MedicationId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required]
        public MedicationType Type { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative number.")]
        public int Quantity { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Expiry Date")]
        public DateTime ExpiryDate { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Required]
        public MedicationSchedule Schedule { get; set; }

        [Required]
        public MedicationStatus DeletionStatus { get; set; }

        // Helper method to determine if medication is scheduled
        public bool IsScheduledMedication()
        {
            return Schedule >= MedicationSchedule.Schedule5;
        }

        public bool IsNonScheduledMedication()
        {
            return Schedule <= MedicationSchedule.Schedule4;
        }
    }

    public enum ScheduledMedicationStatus
    {
        Active,
        Completed,
        Cancelled,
        Deleted
    }

    public enum NonScheduledMedicationStatus
    {
        Active,
        Deleted
    }

    public enum MedicationType
    {
        [Display(Name = "Prescription Medication")]
        Prescription,
        [Display(Name = "Over the Counter")]
        OverTheCounter,
        [Display(Name = "Supplement")]
        Supplement,
        [Display(Name = "Other")]
        Other
    }

    public enum MedicationStatus
    {
        [Display(Name = "Active")]
        Active,
        [Display(Name = "Deleted")]
        Deleted
    }

    public enum MedicationSchedule
    {
        [Display(Name = "Schedule 1")]
        Schedule1,
        [Display(Name = "Schedule 2")]
        Schedule2,
        [Display(Name = "Schedule 3")]
        Schedule3,
        [Display(Name = "Schedule 4")]
        Schedule4,
        [Display(Name = "Schedule 5")]
        Schedule5,
        [Display(Name = "Schedule 6")]
        Schedule6,
        [Display(Name = "Schedule 7")]
        Schedule7
    }

    // Custom validation attributes
    public class FutureOrTodayDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date.Date < DateTime.Today)
                {
                    return new ValidationResult(ErrorMessage ?? "The date must be today or a future date.");
                }
            }
            return ValidationResult.Success;
        }
    }

    public class TodayDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date.Date != DateTime.Today)
                {
                    return new ValidationResult(ErrorMessage ?? "The date must be today's date.");
                }
            }
            return ValidationResult.Success;
        }
    }
}



    