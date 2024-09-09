using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Hospital_Management_System_ASP.NET.Models;
using Hospital_Management_System_ASP.NET.ViewModels;
using Microsoft.AspNet.Identity;

namespace Hospital_Management_System_ASP.NET.Controllers
{
    public class PatientsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return RedirectToAction("UpdateProfile");
        }
        //UPDATE PROFILE PART
        [Authorize(Roles = "Patient")]
        public ActionResult UpdateProfile()
        {
            string id=User.Identity.GetUserId();
            var patient = db.Patients.Single(c => c.ApplicationUserId == id);
            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(Patient model)
        {
            string id = User.Identity.GetUserId();
            var patient = db.Patients.Single(c => c.ApplicationUserId == id);
            patient.FirstName = model.FirstName;
            patient.LastName = model.LastName;
            patient.EmailAddress = model.EmailAddress;
            patient.Address = model.Address;
            patient.BloodGroup = model.BloodGroup;
            patient.DateOfBirth = model.DateOfBirth;
            patient.Gender = model.Gender;
            patient.PhoneNo = model.PhoneNo;
            db.SaveChanges();
            return View();
        }




        //DOCTORS PART

        //GET: Patients/AvailableDoctors
        [Authorize(Roles = "Patient")]
        public ActionResult AvailableDoctors()
        {
            var doctors = db.Doctors.Include(d => d.Department).Where(d => d.Status.Equals("Active"));
            return View(doctors.ToList());
        }


        //GET: Patients/DoctorDetails/1
        [Authorize(Roles = "Patient")]
        public ActionResult DoctorDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Doctor doctor = db.Doctors.Include(d => d.Department)
                                      .FirstOrDefault(d => d.DoctorId == id);

            if (doctor == null)
            {
                return HttpNotFound();
            }

            return View(doctor);
        }

        //APPOINTMENTS PART

        //GET: Patients/AddAppointment
        [Authorize(Roles = "Patient")]
        public ActionResult AddAppointment()
        {
            var collection = new AppointmentViewModel
            {
                Appointment = new Appointment(),
                Doctors = db.Doctors.ToList()
            };
            return View(collection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAppointment(AppointmentViewModel model)
        {
            var collection = new AppointmentViewModel
            {
                Appointment = model.Appointment,
                Doctors = db.Doctors.ToList()
            };
            if (model.Appointment.AppointmentTime >= DateTime.Now.Date)
            {
                string user = User.Identity.GetUserId();
                var patient = db.Patients.Single(c => c.ApplicationUserId == user);
                var appointment = new Appointment();
                appointment.PatientId = patient.PatientId;
                appointment.DoctorId = model.Appointment.DoctorId;
                appointment.AppointmentTime = model.Appointment.AppointmentTime;
                appointment.Problem = model.Appointment.Problem;
                appointment.Status = false;

                db.Appointments.Add(appointment);
                db.SaveChanges();
                return RedirectToAction("ListOfAppointments");
            }
            ViewBag.Messege = "Please Enter the Date greater than today or equal!!";

            return View(collection);

        }

        //GET: Patients/ListOfAppointments
        [Authorize(Roles = "Patient")]
        public ActionResult ListOfAppointments()
        {
            string user = User.Identity.GetUserId();
            var patient = db.Patients.Single(c => c.ApplicationUserId == user);
            var appointments = db.Appointments.Include(c => c.Doctor).Where(c => c.PatientId == patient.PatientId).ToList();
            return View(appointments);
        }


        //GET: Patients/EditAppointment/1
        [Authorize(Roles = "Patient")]
        public ActionResult EditAppointment(int id)
        {
            var collection = new AppointmentViewModel
            {
                Appointment = db.Appointments.Single(c => c.AppointmentId == id),
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
                Doctors = db.Doctors.ToList()
            };
            if (model.Appointment.AppointmentTime >= DateTime.Now.Date)
            {
                var appointment = db.Appointments.Single(c => c.AppointmentId == id);
                appointment.DoctorId = model.Appointment.DoctorId;
                appointment.AppointmentTime = model.Appointment.AppointmentTime;
                appointment.Problem = model.Appointment.Problem;
                db.SaveChanges();
                return RedirectToAction("ListOfAppointments");
            }
            ViewBag.Messege = "Please Enter the Date greater than today or equal!!";

            return View(collection);
        }


        //GET: Patients/DeleteAppointment/1
        [Authorize(Roles = "Patient")]
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
            return RedirectToAction("ListOfAppointments");
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
