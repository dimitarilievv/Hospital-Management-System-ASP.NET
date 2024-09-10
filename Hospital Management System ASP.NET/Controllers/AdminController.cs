using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Hospital_Management_System_ASP.NET.Models;
using Hospital_Management_System_ASP.NET.ViewModels;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Net.Mail;


namespace Hospital_Management_System_ASP.NET.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db;

        private ApplicationUserManager _userManager;

        //Constructor
        public AdminController()
        {
            db = new ApplicationDbContext();
        }

        //Destructor
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }
        public ActionResult Index()
        {
            return View();
        }

        //Deparments part

        //All departments
        [Authorize(Roles = "Admin")]
        public ActionResult DepartmentList()
        {
            var model = db.Departments.ToList();
            return View(model);
        }

        //Add Department
        [Authorize(Roles = "Admin")]
        public ActionResult AddDepartment()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDepartment(Department model)
        {
            if (db.Departments.Any(c => c.Name == model.Name))
            {
                ModelState.AddModelError("Name", "Name already present!");
                return View(model);
            }

            db.Departments.Add(model);
            db.SaveChanges();
            return RedirectToAction("DepartmentList");
        }
        //Edit Department
        [Authorize(Roles = "Admin")]
        public ActionResult EditDepartment(int id)
        {
            var model = db.Departments.SingleOrDefault(c => c.DepartmentId == id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDepartment(int id, Department model)
        {
            var department = db.Departments.Single(c => c.DepartmentId == id);
            department.Name = model.Name;
            department.Description = model.Description;
            department.Status = model.Status;
            db.SaveChanges();
            return RedirectToAction("DepartmentList");
        }

        //Delete Department
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteDepartment(int? id)
        {
            var department = db.Departments.Single(c => c.DepartmentId == id);
            return View(department);
        }

        [HttpPost, ActionName("DeleteDepartment")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDepartment(int id)
        {
            var department = db.Departments.SingleOrDefault(c => c.DepartmentId == id);
            db.Departments.Remove(department);
            db.SaveChanges();
            return RedirectToAction("DepartmentList");
        }

        //Doctors part

        //List Of Doctors
        [Authorize(Roles = "Admin")]
        public ActionResult ListOfDoctors()
        {
            var doctors = db.Doctors.Include(d => d.Department).ToList();
            return View(doctors);
        }

        //Add Doctor 
        [Authorize(Roles = "Admin")]
        public ActionResult AddDoctor()
        {
            var collection = new DoctorAndDepartmentViewModel
            {
                ApplicationUser = new RegisterViewModel(),
                Doctor = new Doctor(),
                Departments = db.Departments.ToList()
            };
            return View(collection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddDoctor(DoctorAndDepartmentViewModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.ApplicationUser.Email,
                Email = model.ApplicationUser.Email
            };
            var result = await UserManager.CreateAsync(user, model.ApplicationUser.Password);
            if (result.Succeeded)
            {
                await UserManager.AddToRoleAsync(user.Id, "Doctor");
                var doctor = new Doctor
                {
                    FirstName = model.Doctor.FirstName,
                    LastName = model.Doctor.LastName,
                    FullName = "Dr. " + model.Doctor.FirstName + " " + model.Doctor.LastName,
                    EmailAddress = model.ApplicationUser.Email,
                    PhoneNo = model.Doctor.PhoneNo,
                    DepartmentId = model.Doctor.DepartmentId,
                    Specialization = model.Doctor.Specialization,
                    Gender = model.Doctor.Gender,
                    ImageURL = model.Doctor.ImageURL,
                    Address = model.Doctor.Address,
                    ApplicationUserId = user.Id,
                    DateOfBirth = model.Doctor.DateOfBirth,
                    Status = model.Doctor.Status
                };
                db.Doctors.Add(doctor);
                db.SaveChanges();
                return RedirectToAction("ListOfDoctors");
            }

            return HttpNotFound();

        }
        //Detail of Doctor
        [Authorize(Roles = "Admin")]
        public ActionResult DoctorDetail(int id)
        {
            var doctor = db.Doctors.Include(c => c.Department).SingleOrDefault(c => c.DoctorId == id);
            return View(doctor);
        }

        //Edit Doctors
        [Authorize(Roles = "Admin")]
        public ActionResult EditDoctors(int id)
        {
            var collection = new DoctorAndDepartmentViewModel
            {
                Departments = db.Departments.ToList(),
                Doctor = db.Doctors.Single(c => c.DoctorId == id)
            };
            return View(collection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDoctors(int id, DoctorAndDepartmentViewModel model)
        {
            var doctor = db.Doctors.Single(c => c.DoctorId == id);
            doctor.FirstName = model.Doctor.FirstName;
            doctor.LastName = model.Doctor.LastName;
            doctor.FullName = "Dr. " + model.Doctor.FirstName + " " + model.Doctor.LastName;
            doctor.PhoneNo = model.Doctor.PhoneNo;
            doctor.DepartmentId = model.Doctor.DepartmentId;
            doctor.Specialization = model.Doctor.Specialization;
            doctor.Gender = model.Doctor.Gender;
            doctor.ImageURL = model.Doctor.ImageURL;
            doctor.Address = model.Doctor.Address;
            doctor.DateOfBirth = model.Doctor.DateOfBirth;
            doctor.Status = model.Doctor.Status;
           

            db.SaveChanges();

            return RedirectToAction("ListOfDoctors");
        }
        //if you want to delete doctor by doctorid
        public ActionResult DeleteFirst4(int id)
        {
            var Doctor=db.Doctors.Single(d=>d.DoctorId==id);
            db.Doctors.Remove(Doctor);
            db.SaveChanges();
            return RedirectToAction("ListOfDoctors");
        }
        //Delete Doctor
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteDoctor(string id)
        {
            var UserId = db.Doctors.Single(c => c.ApplicationUserId == id);
            return View(UserId);
        }

        [HttpPost, ActionName("DeleteDoctor")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDoctor(string id, Doctor model)
        {
            var doctor = db.Doctors.Single(c => c.ApplicationUserId == id);
            var user = db.Users.Single(c => c.Id == id);
            if (db.Schedules.Where(c => c.DoctorId == doctor.DoctorId).Equals(null))
            {
                var schedule = db.Schedules.Single(c => c.DoctorId == doctor.DoctorId);
                db.Schedules.Remove(schedule);
            }

            db.Users.Remove(user);
            db.Doctors.Remove(doctor);
            db.SaveChanges();
            return RedirectToAction("ListOfDoctors");
        }



    }
}
