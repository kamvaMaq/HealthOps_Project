using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class InsuranceProvider
    {

        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
