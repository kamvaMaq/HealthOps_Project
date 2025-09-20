using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class PatientMovement
    {
        [Key]
        public int MovementId { get; set; }

        [Required]
        public DateTime MovementDate { get; set; }

        [Required]
        [StringLength(100)]
        public string MovementType { get; set; } // e.g. Transfer, Admission

        [Required]
        [ForeignKey(nameof(Patient))]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Room))]
        public int RoomId { get; set; }

        // ✅ Soft delete support
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public Patient Patient { get; set; }
        public Room Room { get; set; }
    }
}
