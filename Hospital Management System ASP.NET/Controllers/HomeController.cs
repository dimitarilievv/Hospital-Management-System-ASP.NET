using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using Hospital_Management_System_ASP.NET.Models;
using System.Data.Entity;


namespace Hospital_Management_System_ASP.NET.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            var model = new DoctorAndDepartmentModelView
            {
                Doctors = db.Doctors.ToList(),
                Departments = db.Departments.ToList()
            };
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}