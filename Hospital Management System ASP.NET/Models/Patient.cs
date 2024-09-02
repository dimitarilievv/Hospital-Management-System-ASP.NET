using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace Hospital_Management_System_ASP.NET.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        [EmailAddress]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }
        [Display(Name = "Phone No")]
        public string PhoneNo { get; set; }
        [Display(Name = "Blood Group")]
        public string BloodGroup { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Doctor> Doctors { get; set; }
    }
}