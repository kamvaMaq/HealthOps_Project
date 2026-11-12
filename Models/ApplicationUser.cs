using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthOps_Project.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [NotMapped] 
        public string FullName => $"{FirstName} {LastName}";

        [Required]
        public UserRole Role { get; set; }

        [Required]
        [StringLength(50)]
        public string StaffId { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public DateTime DateRegistered { get; set; } = DateTime.UtcNow;


    }



    public enum UserRole
    {
        Nurse,
        NursingSister,
        ScriptManager,  
        StockManager,
        Admin,
        Doctor
    }
}

