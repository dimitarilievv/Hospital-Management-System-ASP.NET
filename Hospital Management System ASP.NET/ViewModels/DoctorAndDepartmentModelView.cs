using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hospital_Management_System_ASP.NET.Models
{
    public class DoctorAndDepartmentModelView
    {
        public IEnumerable<Doctor> Doctors { get; set; }
        public IEnumerable<Department> Departments { get; set; }
    }
}