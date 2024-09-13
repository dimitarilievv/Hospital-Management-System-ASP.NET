using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Hospital_Management_System_ASP.NET.Models;
using Hospital_Management_System_ASP.NET.ViewModels;
using Microsoft.AspNet.Identity;

namespace Hospital_Management_System_ASP.NET.Controllers
{
    public class DoctorsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //Update profile
        [Authorize(Roles = "Doctor")]
        public ActionResult UpdateProfile()
        {
            string id = User.Identity.GetUserId();
            var doctor = db.Doctors.Single(c => c.ApplicationUserId == id);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", doctor.DepartmentId);
            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(Doctor model)
        {
            string id = User.Identity.GetUserId();
            var doctor = db.Doctors.Single(c => c.ApplicationUserId == id);

            if (ModelState.IsValid)
            {
                doctor.FirstName = model.FirstName;
                doctor.LastName = model.LastName;
                doctor.EmailAddress = model.EmailAddress;
                doctor.Address = model.Address;
                doctor.Specialization = model.Specialization;
                doctor.ImageURL = model.ImageURL;
                doctor.DateOfBirth = model.DateOfBirth;
                doctor.Gender = model.Gender;
                doctor.PhoneNo = model.PhoneNo;
                doctor.Status = model.Status;
                doctor.DepartmentId = model.DepartmentId;

                db.SaveChanges();
                return RedirectToAction("UpdateProfile");
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", doctor.DepartmentId);
            return View(model);
        }

        [Authorize(Roles = "Doctor")]
        public ActionResult ScheduleDetail()
        {
            string user = User.Identity.GetUserId();
            var doctor = db.Doctors.Single(c => c.ApplicationUserId == user);
            Schedule schedule = db.Schedules.Single(c => c.DoctorId == doctor.DoctorId);

            if (schedule == null)
            {
                return HttpNotFound("This doctor hasn't set up shedule"); // Handle cases where schedule is not found
            }

            return View(schedule);

        }

        //Edit Schedule
        [Authorize(Roles = "Doctor")]
        public ActionResult EditSchedule(int id)
        {
            var schedule = db.Schedules.Single(c => c.Id == id);
            return View(schedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSchedule(int id, Schedule model)
        {
            var schedule = db.Schedules.Single(c => c.Id == id);
            schedule.AvailableEndDay = model.AvailableEndDay;
            schedule.AvailableEndTime = model.AvailableEndTime;
            schedule.AvailableStartDay = model.AvailableStartDay;
            schedule.AvailableStartTime = model.AvailableStartTime;
            schedule.Status = model.Status;
            schedule.TimePerPatient = model.TimePerPatient;
            db.SaveChanges();
            return RedirectToAction("ScheduleDetail");
        }

        //Add appointment
        [Authorize(Roles = "Doctor")]
        public ActionResult AddAppointment()
        {
            var collection = new AppointmentViewModel
            {
                Appointment = new Appointment(),
                Patients = db.Patients.ToList()
            };
            return View(collection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAppointment(AppointmentViewModel model)
        {
            string user = User.Identity.GetUserId();

            // Get the role IDs for the "Patient" and "Admin" roles
            var patientRoleId = db.Roles.SingleOrDefault(r => r.Name == "Patient").Id;
            var adminRoleId = db.Roles.SingleOrDefault(r => r.Name == "Admin").Id;

            // Fetch only users who have the "Patient" role but do not have the "Admin" role
            var patients = db.Users
                             .Where(u => u.Roles.Any(r => r.RoleId == patientRoleId) && !u.Roles.Any(r => r.RoleId == adminRoleId))
                             .Join(db.Patients, u => u.Id, p => p.ApplicationUserId, (u, p) => p)
                             .ToList();

            var collection = new AppointmentViewModel
            {
                Appointment = model.Appointment,
                Patients = patients
            };
            if (model.Appointment.AppointmentTime >= DateTime.Now.Date)
            {
                var doctor = db.Doctors.Single(c => c.ApplicationUserId == user);
                var appointment = new Appointment();
                appointment.PatientId = model.Appointment.PatientId;
                appointment.DoctorId = doctor.DoctorId;
                appointment.AppointmentTime = model.Appointment.AppointmentTime;
                appointment.Problem = model.Appointment.Problem;
                appointment.Status = model.Appointment.Status;

                db.Appointments.Add(appointment);
                db.SaveChanges();

                if (model.Appointment.Status == true)
                {
                    return RedirectToAction("ActiveAppointments");
                }
                else
                {
                    return RedirectToAction("PendingAppointments");
                }
            }
            ViewBag.Messege = "Please Enter the Date greater than today or equal!!";

            return View(collection);
        }

        //List of Active Appointments
        [Authorize(Roles = "Doctor")]
        public ActionResult ActiveAppointments()
        {
            var user = User.Identity.GetUserId();
            var doctor = db.Doctors.Single(c => c.ApplicationUserId == user);
            var date = DateTime.Now.Date;
            var appointment = db.Appointments.Include(c => c.Doctor).Include(c => c.Patient).Where(c => c.DoctorId == doctor.DoctorId).Where(c => c.Status ==true).Where(c => c.AppointmentTime >= date).ToList();
            return View(appointment);
        }
        //List of Pending Appointments
        public ActionResult PendingAppointments()
        {
            var user = User.Identity.GetUserId();
            var doctor = db.Doctors.Single(c => c.ApplicationUserId == user);
            var date = DateTime.Now.Date;
            var appointment = db.Appointments.Include(c => c.Doctor).Include(c => c.Patient).Where(c => c.DoctorId == doctor.DoctorId).Where(c => c.Status == false).Where(c => c.AppointmentTime >= date).ToList();
            return View(appointment);
        }

        //Edit Appointment
        [Authorize(Roles = "Doctor")]
        public ActionResult EditAppointment(int id)
        {
            var collection = new AppointmentViewModel
            {
                Appointment = db.Appointments.Single(c => c.AppointmentId == id),
                Patients = db.Patients.ToList()
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
                Patients = db.Patients.ToList()
            };
            if (model.Appointment.AppointmentTime >= DateTime.Now.Date)
            {
                var appointment = db.Appointments.Single(c => c.AppointmentId == id);
                appointment.PatientId = model.Appointment.PatientId;
                appointment.AppointmentTime = model.Appointment.AppointmentTime;
                appointment.Problem = model.Appointment.Problem;
                appointment.Status = model.Appointment.Status;
                db.SaveChanges();
                if (model.Appointment.Status == true)
                {
                    return RedirectToAction("ActiveAppointments");
                }
                else
                {
                    return RedirectToAction("PendingAppointments");
                }
            }
            ViewBag.Messege = "Please Enter the Date greater than today or equal!!";

            return View(collection);
        }

        //Detail of appointment
        [Authorize(Roles = "Doctor")]
        public ActionResult DetailOfAppointment(int id)
        {
            var appointment = db.Appointments.Include(c => c.Doctor).Include(c => c.Patient).Single(c => c.AppointmentId == id);
            return View(appointment);
        }

        //Delete Appointment
        [Authorize(Roles = "Doctor")]
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
                return RedirectToAction("ActiveAppointments");
            }
            else
            {
                return RedirectToAction("PendingAppointments");
            }
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
