using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System_ASP.NET.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Status { get; set; }

        public ICollection<Doctor> Doctors { get; set; }
    }
}