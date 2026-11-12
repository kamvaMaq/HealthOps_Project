using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace HealthOps_Project.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(13)]
        [Display(Name = "ID Number")]
        public string IdNumber { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [StringLength(20)]
        [Display(Name = "Emergency Contact")]
        public string EmergencyContact { get; set; }

        // Changed from RoleId to Role enum
        [Required]
        [Display(Name = "Role")]
        public EmployeeRoleType Role { get; set; }

        // Keep these for foreign key relationships (optional)
        [Display(Name = "Department")]
        public int? DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        [Display(Name = "Ward")]
        public int? WardId { get; set; }

        [ForeignKey("WardId")]
        public virtual Ward Ward { get; set; }

        // Manual text input fields
        [StringLength(100)]
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }

        [StringLength(100)]
        [Display(Name = "Ward Name")]
        public string WardName { get; set; }

        [StringLength(200)]
        public string Qualifications { get; set; }

        [StringLength(100)]
        [Display(Name = "License Number")]
        public string LicenseNumber { get; set; }

        [StringLength(200)]
        public string Specialization { get; set; }

        [Required]
        [Display(Name = "Employment Status")]
        public EmploymentStatus Status { get; set; }

        [Required]
        [Display(Name = "Hire Date")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        [Display(Name = "Termination Date")]
        [DataType(DataType.Date)]
        public DateTime? TerminationDate { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        [Required]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";
    }

    // Employee Role Enum - replaces the EmployeeRole class
    public enum EmployeeRoleType
    {
        [Display(Name = "Doctor")]
        Doctor,

        [Display(Name = "Ward Admin")]
        WardAdmin,

        [Display(Name = "Nurse")]
        Nurse,

        [Display(Name = "Nursing Sister")]
        NursingSister,

        [Display(Name = "Script Manager")]
        ScriptManager,

        [Display(Name = "Stock Manager")]
        StockManager
    }

    public enum EmploymentStatus
    {
        Active,
        OnLeave,
        Suspended,
        Terminated,
        Retired
    }

    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<Ward> Wards { get; set; }
    }
}