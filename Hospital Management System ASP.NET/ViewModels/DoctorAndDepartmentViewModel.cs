using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hospital_Management_System_ASP.NET.Models;

namespace Hospital_Management_System_ASP.NET.ViewModels
{
    public class DoctorAndDepartmentViewModel
    {
        public RegisterViewModel ApplicationUser { get; set; }
        public Doctor Doctor { get; set; }
        public IEnumerable<Department> Departments { get; set; }
    }
}