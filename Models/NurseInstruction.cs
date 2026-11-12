namespace HealthOps_Project.Models
{
    using System.ComponentModel.DataAnnotations;

    public class NurseInstruction
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        public Patient? Patient { get; set; }

        [Required(ErrorMessage = "Instruction/Notes cannot be empty.")]
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string? Instruction { get; set; }

        [Required(ErrorMessage = "Nurse Name is required.")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string NurseName { get; set; }


        [Required]
        public DateTime IssuedAt { get; set; } = DateTime.Now;

        public bool IsCompleted { get; set; } 

        public bool isActive { get; set; } = true;
    }

}