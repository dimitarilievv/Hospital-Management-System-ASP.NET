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
using System.Net;

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
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string message)
        {
            var date = DateTime.Now.Date;
            ViewBag.Messege = message;
            var model = new AllModelsViewModel
            {
                Departments = db.Departments.ToList(),
                Doctors = db.Doctors.ToList(),
                Patients = db.Patients.ToList(),
                ActiveAppointments =
                    db.Appointments.Where(c => c.Status).Where(c => c.AppointmentTime >= date).ToList(),
                PendingAppointments = db.Appointments.Where(c => c.Status == false)
                    .Where(c => c.AppointmentTime >= date).ToList(),
            };
            return View(model);
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
            // Check if a user with the same email already exists
            var existingUser = await UserManager.FindByEmailAsync(model.ApplicationUser.Email);

            if (existingUser != null)
            {
              
                return View(model);  // Return the view with the entered data
            }

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
                await db.SaveChangesAsync();

                // Create default schedule for the doctor
                var schedule = new Schedule
                {
                    DoctorId = doctor.DoctorId,
                    AvailableStartDay = "Monday",  // Default start day
                    AvailableEndDay = "Friday",    // Default end day
                    AvailableStartTime = DateTime.Today.AddHours(9), // Default start time: 09:00 AM
                    AvailableEndTime = DateTime.Today.AddHours(17),  // Default end time: 05:00 PM
                    TimePerPatient = "30", // Default time per patient
                    Status = "Active"           // Default status
                };

                db.Schedules.Add(schedule);
                await db.SaveChangesAsync();

                return RedirectToAction("ListOfDoctors");
            }

            return View(model);  // Return the view with errors
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
            var doctor = db.Doctors.Include(d => d.ApplicationUser).FirstOrDefault(d => d.DoctorId == id);
            if (doctor == null)
            {
                return HttpNotFound();
            }

            var model = new DoctorAndDepartmentViewModel
            {
                Doctor = doctor,
                ApplicationUser = new RegisterViewModel
                {
                    Email = doctor.ApplicationUser.Email
                },
                Departments = db.Departments.ToList()
            };

            return View(model);
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

        //List of Patients
        [Authorize(Roles = "Admin")]
        public ActionResult ListOfPatients()
        {
            var patientRoleId = db.Roles.SingleOrDefault(r => r.Name == "Patient").Id;
            var patients = db.Users
                             .Where(u => u.Roles.Any(r => r.RoleId == patientRoleId))
                             .Join(db.Patients, u => u.Id, p => p.ApplicationUserId, (u, p) => p)
                             .ToList();
            return View(patients);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditPatient(int id)
        {
            var patient = db.Patients.Single(c => c.PatientId == id);
            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPatient(int id, Patient model)
        {
            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var patient = db.Patients.Single(c => c.PatientId == id);
            patient.FirstName = model.FirstName;
            patient.LastName = model.LastName;
            patient.FullName = model.FirstName + " " + model.LastName;
            patient.Address = model.Address;
            patient.BloodGroup = model.BloodGroup;
            patient.DateOfBirth = model.DateOfBirth;
            patient.EmailAddress = model.EmailAddress;
            patient.Gender = model.Gender;
            patient.PhoneNo = model.PhoneNo;
            db.SaveChanges();
            return RedirectToAction("ListOfPatients");
        }

        //Delete Patient
        [Authorize(Roles = "Admin")]
        public ActionResult DeletePatient()
        {
            return View();
        }

        [HttpPost, ActionName("DeletePatient")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePatient(string id)
        {
            var patient = db.Patients.Single(c => c.ApplicationUserId == id);
            var user = db.Users.Single(c => c.Id == id);
            db.Users.Remove(user);
            db.Patients.Remove(patient);
            db.SaveChanges();
            return RedirectToAction("ListOfPatients");
        }

        //Add Schedule
        [Authorize(Roles = "Admin")]
        public ActionResult AddSchedule()
        {
            var collection = new ScheduleDoctorViewModel
            {
                Schedule = new Schedule(),
                Doctors = db.Doctors.ToList()
            };
            return View(collection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSchedule(ScheduleDoctorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var collection = new ScheduleDoctorViewModel
                {
                    Schedule = model.Schedule,
                    Doctors = db.Doctors.ToList()
                };
                return View(collection);
            }

            db.Schedules.Add(model.Schedule);
            db.SaveChanges();
            return RedirectToAction("ListOfSchedules");
        }

        //List Of Schedules
        [Authorize(Roles = "Admin")]
        public ActionResult ListOfSchedules()
        {
            var schedule = db.Schedules.Include(c => c.Doctor).ToList();
            return View(schedule);
        }

        //Edit Schedule
        [Authorize(Roles = "Admin")]
        public ActionResult EditSchedule(int id)
        {
            var collection = new ScheduleDoctorViewModel
            {
                Schedule = db.Schedules.Single(c => c.Id == id),
                Doctors = db.Doctors.ToList()
            };
            return View(collection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSchedule(int id, ScheduleDoctorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var schedule = db.Schedules.Single(c => c.Id == id);
            schedule.DoctorId = model.Schedule.DoctorId;
            schedule.AvailableEndDay = model.Schedule.AvailableEndDay;
            schedule.AvailableEndTime = model.Schedule.AvailableEndTime;
            schedule.AvailableStartDay = model.Schedule.AvailableStartDay;
            schedule.AvailableStartTime = model.Schedule.AvailableStartTime;
            schedule.Status = model.Schedule.Status;
            schedule.TimePerPatient = model.Schedule.TimePerPatient;
            db.SaveChanges();
            return RedirectToAction("ListOfSchedules");
        }

        //Delete Schedule
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteSchedule(int? id)
        {
            return View();
        }

        [HttpPost, ActionName("DeleteSchedule")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSchedule(int id)
        {
            var schedule = db.Schedules.Single(c => c.Id == id);
            db.Schedules.Remove(schedule);
            return RedirectToAction("ListOfSchedules");
        }

        //List of Active Appointment
        [Authorize(Roles = "Admin")]
        public ActionResult ListOfAppointments()
        {
            var date = DateTime.Now.Date;
            var appointment = db.Appointments.Include(c => c.Doctor).Include(c => c.Patient)
                .Where(c => c.Status == true).Where(c => c.AppointmentTime >= date).ToList();
            return View(appointment);
        }

        //List of pending Appointments
        [Authorize(Roles = "Admin")]
        public ActionResult PendingAppointments()
        {
            var date = DateTime.Now.Date;
            var appointment = db.Appointments.Include(c => c.Doctor).Include(c => c.Patient)
                .Where(c => c.Status == false).Where(c => c.AppointmentTime >= date).ToList();
            return View(appointment);
        }
        //Add Appointment
        [Authorize(Roles = "Admin")]
        public ActionResult AddAppointment()
        {
            var patientRoleId = db.Roles.SingleOrDefault(r => r.Name == "Patient").Id;
            var patients = db.Users
                             .Where(u => u.Roles.Any(r => r.RoleId == patientRoleId))
                             .Join(db.Patients, u => u.Id, p => p.ApplicationUserId, (u, p) => p)
                             .ToList();
            var doctorRoleId = db.Roles.SingleOrDefault(r => r.Name == "Doctor").Id;
            var doctors = db.Users
                             .Where(u => u.Roles.Any(r => r.RoleId == doctorRoleId))
                             .Join(db.Doctors, u => u.Id, p => p.ApplicationUserId, (u, p) => p)
                             .ToList();
            var collection = new AppointmentViewModel
            {
                Appointment = new Appointment(),
                Patients = patients,
                Doctors = doctors
            };
            return View(collection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAppointment(AppointmentViewModel model)
        {
            var patientRoleId = db.Roles.SingleOrDefault(r => r.Name == "Patient").Id;
            var patients = db.Users
                             .Where(u => u.Roles.Any(r => r.RoleId == patientRoleId))
                             .Join(db.Patients, u => u.Id, p => p.ApplicationUserId, (u, p) => p)
                             .ToList();
            var doctorRoleId = db.Roles.SingleOrDefault(r => r.Name == "Doctor").Id;
            var doctors = db.Users
                             .Where(u => u.Roles.Any(r => r.RoleId == doctorRoleId))
                             .Join(db.Doctors, u => u.Id, p => p.ApplicationUserId, (u, p) => p)
                             .ToList();
            var collection = new AppointmentViewModel
            {
                Appointment = model.Appointment,
                Patients = patients,
                Doctors = doctors
            };
            if (model.Appointment.AppointmentTime >= DateTime.Now.Date)
            {
                var appointment = new Appointment();
                appointment.PatientId = model.Appointment.PatientId;
                appointment.DoctorId = model.Appointment.DoctorId;
                appointment.AppointmentTime = model.Appointment.AppointmentTime;
                appointment.Problem = model.Appointment.Problem;
                appointment.Status = model.Appointment.Status;
                db.Appointments.Add(appointment);
                db.SaveChanges();

                if (model.Appointment.Status == true)
                {
                    return RedirectToAction("ListOfAppointments");
                }
                else
                {
                    return RedirectToAction("PendingAppointments");
                }
            }

            ViewBag.Messege = "Please Enter the Date greater than today or equal!!";
            return View(collection);

        }

        //Edit Appointment
        [Authorize(Roles = "Admin")]
        public ActionResult EditAppointment(int id)
        {
            var collection = new AppointmentViewModel
            {
                Appointment = db.Appointments.Single(c => c.AppointmentId == id),
                Patients = db.Patients.ToList(),
                Doctors = db.Doctors.ToList()
            };
            return View(collection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAppointment(int id, AppointmentViewModel model)
        {
            var collection = new AppointmentViewModel
            {
                Appointment = model.Appointment,
                Patients = db.Patients.ToList(),
                Doctors = db.Doctors.ToList()
            };
            if (model.Appointment.AppointmentTime >= DateTime.Now.Date)
            {
                var appointment = db.Appointments.Single(c => c.AppointmentId == id);
                appointment.PatientId = model.Appointment.PatientId;
                appointment.DoctorId = model.Appointment.DoctorId;
                appointment.AppointmentTime = model.Appointment.AppointmentTime;
                appointment.Problem = model.Appointment.Problem;
                appointment.Status = model.Appointment.Status;
                db.SaveChanges();
                if (model.Appointment.Status == true)
                {
                    return RedirectToAction("ListOfAppointments");
                }
                else
                {
                    return RedirectToAction("PendingAppointments");
                }
            }
            ViewBag.Messege = "Please Enter the Date greater than today or equal!!";

            return View(collection);
        }

        //Detail of Appointment
        [Authorize(Roles = "Admin")]
        public ActionResult DetailOfAppointment(int id)
        {
            var appointment = db.Appointments.Include(c => c.Doctor).Include(c => c.Patient).Single(c => c.AppointmentId == id);
            return View(appointment);
        }

        //Delete Appointment
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteAppointment(int? id)
        {
            var appointment = db.Appointments.Single(c => c.AppointmentId == id);
            return View(appointment);
        }

        [HttpPost, ActionName("DeleteAppointment")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAppointment(int id)
        {
            var appointment = db.Appointments.Single(c => c.AppointmentId == id);
            db.Appointments.Remove(appointment);
            db.SaveChanges();
            if (appointment.Status)
            {
                return RedirectToAction("ListOfAppointments");
            }
            else
            {
                return RedirectToAction("PendingAppointments");
            }
        }


    }
}
