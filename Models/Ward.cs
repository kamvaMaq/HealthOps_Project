using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthOps_Project.Models
{
    public class Ward
    {
        [Key]
        public int WardId { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Location { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Bed>? Beds { get; set; }


        public ICollection<Admission>? Admissions { get; set; }


        // Calculated properties (not stored in database)
        [NotMapped]
        public int CurrentCapacity => Beds?.Count(b => !b.IsAvailable) ?? 0;

        [NotMapped]
        public int TotalBeds => Beds?.Count ?? 0;

        [NotMapped]
        public int OccupiedBeds => CurrentCapacity;

        [NotMapped]
        public int AvailableBeds => TotalBeds - OccupiedBeds;

        [NotMapped]
        public double OccupancyRate => TotalBeds > 0 ? (double)OccupiedBeds / TotalBeds * 100 : 0;

        [NotMapped]
        public string Status
        {
            get
            {
                if (TotalBeds == 0) return "No Beds";
                if (OccupiedBeds == 0) return "Empty";
                if (AvailableBeds == 0) return "Full";
                if (OccupiedBeds >= TotalBeds * 0.8) return "High Occupancy";
                return "Available";
            }
        }

    }
}
