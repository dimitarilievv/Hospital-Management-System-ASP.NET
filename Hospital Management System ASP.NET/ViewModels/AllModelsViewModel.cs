using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hospital_Management_System_ASP.NET.Models;

namespace Hospital_Management_System_ASP.NET.ViewModels
{
    public class AllModelsViewModel
    {
        public IEnumerable<Department> Departments { get; set; }
        public IEnumerable<Doctor> Doctors { get; set; }
        public IEnumerable<Patient> Patients { get; set; }
        public IEnumerable<Appointment> ActiveAppointments { get; set; }
        public IEnumerable<Appointment> PendingAppointments { get; set; }
        public IEnumerable<Prescription> Prescriptions { get; set; }
    }
}