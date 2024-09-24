using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hospital_Management_System_ASP.NET.Models;

namespace Hospital_Management_System_ASP.NET.ViewModels
{
    public class PrescriptionViewModel
    {
        public Prescription Prescription { get; set; }
        public List<Patient> Patients { get; set; }
    }
}