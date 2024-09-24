using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hospital_Management_System_ASP.NET.Models;

namespace Hospital_Management_System_ASP.NET.ViewModels
{
    public class AppointmentViewModel
    {
        public Appointment Appointment { get; set; }
        public IEnumerable<Patient> Patients { get; set; }
        public IEnumerable<Doctor> Doctors { get; set; }
    }
}