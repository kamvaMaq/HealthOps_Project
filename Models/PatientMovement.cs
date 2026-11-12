using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthOps_Project.Models
{
    public class PatientMovement
    {
        [Key]
        public int MovementId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime MovementDate { get; set; }

        [Required]
        [StringLength(100)]
        public string MovementType { get; set; } // e.g., Admission, Transfer, Theatre, X-Ray, Discharge

        [StringLength(250)]
        public string? ReasonForMovement { get; set; }

        // Soft delete
        public bool IsActive { get; set; } = true;

        // Foreign keys
        [Required]
        public int PatientId { get; set; }

        public int DoctorId { get; set; }

        [Required]
        public int WardId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        public int BedId { get; set; }

        public string? DoctorUserId { get; set; } // optional reference to doctor

        // Navigation properties
        [ForeignKey(nameof(PatientId))]
        public Patient Patient { get; set; }

        [ForeignKey(nameof(WardId))]
        public Ward Ward { get; set; }

        [ForeignKey(nameof(RoomId))]
        public Room Room { get; set; }

        [ForeignKey(nameof(BedId))]
        public Bed Bed { get; set; }

        [ForeignKey(nameof(DoctorUserId))]
        public virtual ApplicationUser? DoctorUser { get; set; }
    }
}
