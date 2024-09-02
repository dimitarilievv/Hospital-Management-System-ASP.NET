using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace Hospital_Management_System_ASP.NET.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }
        [Display(Name = "Appointment Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? AppointmentTime { get; set; }
        public string Problem { get; set; }
        public string Status { get; set; }
        public Patient Patient { get; set; }
        [Display(Name = "Patient Name")]
        public int PatientId { get; set; }


        [Display(Name = "Doctor Name")]
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
    }
}