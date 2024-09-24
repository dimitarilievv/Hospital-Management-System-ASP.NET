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
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", model.DepartmentId);

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
            var patientRoleId = db.Roles.SingleOrDefault(r => r.Name == "Patient").Id;
            var patients = db.Users
                             .Where(u => u.Roles.Any(r => r.RoleId == patientRoleId))
                             .Join(db.Patients, u => u.Id, p => p.ApplicationUserId, (u, p) => p)
                             .ToList();
            var collection = new AppointmentViewModel
            {
                Appointment = new Appointment(),
                Patients = patients
            };
            return View(collection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAppointment(AppointmentViewModel model)
        {
            string user = User.Identity.GetUserId();
            var patientRoleId = db.Roles.SingleOrDefault(r => r.Name == "Patient").Id;
            var patients = db.Users
                             .Where(u => u.Roles.Any(r => r.RoleId == patientRoleId))
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

        //PRESCRIPTION PART
        //Add Prescription
        [Authorize(Roles = "Doctor")]
        public ActionResult AddPrescription()
        {
            string user = User.Identity.GetUserId();
            var patientRoleId = db.Roles.SingleOrDefault(r => r.Name == "Patient").Id;
            var patients = db.Users
                             .Where(u => u.Roles.Any(r => r.RoleId == patientRoleId))
                             .Join(db.Patients, u => u.Id, p => p.ApplicationUserId, (u, p) => p)
                             .ToList();
            var collection = new PrescriptionViewModel
            {
                Prescription = new Prescription(),
                Patients = patients
            };
            return View(collection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPrescription(PrescriptionViewModel model)
        {
            string user = User.Identity.GetUserId();
            var patient = db.Patients.Single(c => c.PatientId == model.Prescription.PatientId);
            var doctor = db.Doctors.Single(c => c.ApplicationUserId == user);
            var schedule = db.Schedules.Single(c => c.DoctorId == doctor.DoctorId);
            var patientuser = db.Users.Single(c => c.Id == patient.ApplicationUserId);
            var prescription = new Prescription
            {
                PatientId = model.Prescription.PatientId,
                DoctorId = doctor.DoctorId,
                DoctorName = doctor.FullName,
                DoctorSpecialization = doctor.Specialization,
                PatientName = patient.FullName,
                PatientGender = patient.Gender,
                UserName = patientuser.UserName,
                MedicalTest1 = model.Prescription.MedicalTest1,
                MedicalTest2 = model.Prescription.MedicalTest2,
                MedicalTest3 = model.Prescription.MedicalTest3,
                MedicalTest4 = model.Prescription.MedicalTest4,
                Medicine1 = model.Prescription.Medicine1,
                Medicine2 = model.Prescription.Medicine2,
                Medicine3 = model.Prescription.Medicine3,
                Medicine4 = model.Prescription.Medicine4,
                Medicine5 = model.Prescription.Medicine5,
                Medicine6 = model.Prescription.Medicine6,
                Medicine7 = model.Prescription.Medicine7,
                Morning1 = model.Prescription.Morning1,
                Morning2 = model.Prescription.Morning2,
                Morning3 = model.Prescription.Morning3,
                Morning4 = model.Prescription.Morning4,
                Morning5 = model.Prescription.Morning5,
                Morning6 = model.Prescription.Morning6,
                Morning7 = model.Prescription.Morning7,
                Afternoon1 = model.Prescription.Afternoon1,
                Afternoon2 = model.Prescription.Afternoon2,
                Afternoon3 = model.Prescription.Afternoon3,
                Afternoon4 = model.Prescription.Afternoon4,
                Afternoon5 = model.Prescription.Afternoon5,
                Afternoon6 = model.Prescription.Afternoon6,
                Afternoon7 = model.Prescription.Afternoon7,
                Evening1 = model.Prescription.Evening1,
                Evening2 = model.Prescription.Evening2,
                Evening3 = model.Prescription.Evening3,
                Evening4 = model.Prescription.Evening4,
                Evening5 = model.Prescription.Evening5,
                Evening6 = model.Prescription.Evening6,
                Evening7 = model.Prescription.Evening7,
                CheckUpAfterDays = model.Prescription.CheckUpAfterDays,
                PrescriptionAddDate = DateTime.Now.Date,
                DoctorTiming = "From " + schedule.AvailableStartTime.ToShortTimeString() + " to " + schedule.AvailableEndTime.ToShortTimeString()
            };

            db.Prescriptions.Add(prescription);
            db.SaveChanges();
            return RedirectToAction("ListOfPrescription");
        }

        //List of Prescription
        [Authorize(Roles = "Doctor")]
        public ActionResult ListOfPrescription()
        {
            var user = User.Identity.GetUserId();
            var doctor = db.Doctors.Single(c => c.ApplicationUserId == user);
            var prescription = db.Prescriptions.Where(c => c.DoctorId == doctor.DoctorId).ToList();
            return View(prescription);
        }

        //View Of Prescription
        [Authorize(Roles = "Doctor")]
        public ActionResult ViewPrescription(int id)
        {
            var prescription = db.Prescriptions.Single(c => c.Id == id);
            return View(prescription);
        }

        //Delete Prescription
        [Authorize(Roles = "Doctor")]
        public ActionResult DeletePrescription(int? id)
        {
            return View();
        }

        [HttpPost, ActionName("DeletePrescription")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePrescription(int id)
        {
            var prescription = db.Prescriptions.Single(c => c.Id == id);
            db.Prescriptions.Remove(prescription);
            db.SaveChanges();
            return RedirectToAction("ListOfPrescription");
        }

        //Edit Prescription
        [Authorize(Roles = "Doctor")]
        public ActionResult EditPrescription(int id)
        {
            var prescription = db.Prescriptions.Single(c => c.Id == id);
            return View(prescription);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPrescription(int id, Prescription model)
        {
            var prescription = db.Prescriptions.Single(c => c.Id == id);
            prescription.MedicalTest1 = model.MedicalTest1;
            prescription.MedicalTest2 = model.MedicalTest2;
            prescription.MedicalTest3 = model.MedicalTest3;
            prescription.MedicalTest4 = model.MedicalTest4;
            prescription.Medicine1 = model.Medicine1;
            prescription.Medicine2 = model.Medicine2;
            prescription.Medicine3 = model.Medicine3;
            prescription.Medicine4 = model.Medicine4;
            prescription.Medicine5 = model.Medicine5;
            prescription.Medicine6 = model.Medicine6;
            prescription.Medicine7 = model.Medicine7;
            prescription.Morning1 = model.Morning1;
            prescription.Morning2 = model.Morning2;
            prescription.Morning3 = model.Morning3;
            prescription.Morning4 = model.Morning4;
            prescription.Morning5 = model.Morning5;
            prescription.Morning6 = model.Morning6;
            prescription.Morning7 = model.Morning7;
            prescription.Afternoon1 = model.Afternoon1;
            prescription.Afternoon2 = model.Afternoon2;
            prescription.Afternoon3 = model.Afternoon3;
            prescription.Afternoon4 = model.Afternoon4;
            prescription.Afternoon5 = model.Afternoon5;
            prescription.Afternoon6 = model.Afternoon6;
            prescription.Afternoon7 = model.Afternoon7;
            prescription.Evening1 = model.Evening1;
            prescription.Evening2 = model.Evening2;
            prescription.Evening3 = model.Evening3;
            prescription.Evening4 = model.Evening4;
            prescription.Evening5 = model.Evening5;
            prescription.Evening6 = model.Evening6;
            prescription.Evening7 = model.Evening7;
            prescription.CheckUpAfterDays = model.CheckUpAfterDays;
            db.SaveChanges();
            return RedirectToAction("ListOfPrescription");
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
