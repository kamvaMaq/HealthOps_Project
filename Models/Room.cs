using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthOps_Project.Models
{

    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        [Required]
        [StringLength(20)]
        public string RoomNumber { get; set; }

        [Required]
        public int Capacity { get; set; }

        public bool IsAvailable { get; set; } = true;

        // Navigation Properties
        public ICollection<PatientMovement> PatientMovements { get; set; }
    }

}
