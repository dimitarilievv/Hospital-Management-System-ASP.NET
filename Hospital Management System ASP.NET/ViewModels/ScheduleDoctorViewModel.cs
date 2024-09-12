using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hospital_Management_System_ASP.NET.Models;

namespace Hospital_Management_System_ASP.NET.ViewModels
{
    public class ScheduleDoctorViewModel
    {
        public Schedule Schedule { get; set; }
        public IEnumerable<Doctor> Doctors { get; set; }
    }
}