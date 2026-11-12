using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class Bed
    {
        [Key]
        public int BedId { get; set; }

        [Required, StringLength(10)]
        public string BedNumber { get; set; } = string.Empty;

        [Required]
        public bool IsAvailable { get; set; }

        // ? Patient relationship
        public int? PatientId { get; set; }
        [ForeignKey(nameof(PatientId))]
        public Patient? Patient { get; set; }


        // ? Proper FK + Navigation for Room
        public int? RoomId { get; set; }
        [ForeignKey(nameof(RoomId))]
        public Room? Room { get; set; }

        // Nullable to prevent cascade path issues
        public int? WardId { get; set; }

        [ForeignKey(nameof(WardId))]
        public Ward? Ward { get; set; }

        public ICollection<Admission>? Admissions { get; set; }
    }
}
