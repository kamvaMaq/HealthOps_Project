using Microsoft.AspNetCore.Identity;

namespace HealthOps_Project.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
