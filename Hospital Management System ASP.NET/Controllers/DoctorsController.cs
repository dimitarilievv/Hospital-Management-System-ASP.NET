using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Hospital_Management_System_ASP.NET.Models;
using Microsoft.AspNet.Identity;

namespace Hospital_Management_System_ASP.NET.Controllers
{
    public class DoctorsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Doctors
        [Authorize(Roles = "Doctor")]
        public ActionResult Index()
        {
            var doctors = db.Doctors.Include(d => d.Department);
            return View(doctors.ToList());
        }

        // GET: Doctors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        // GET: Doctors/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name");
            return View();
        }

        // POST: Doctors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DoctorId,FirstName,FullName,LastName,EmailAddress,Gender,DateOfBirth,Address,PhoneNo,Specialization,DepartmentId,ImageURL,Status")] Doctor doctor)
        {
            doctor.FullName = doctor.FirstName + " " + doctor.LastName;
            if (ModelState.IsValid)
            {
                db.Doctors.Add(doctor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", doctor.DepartmentId);
            
            return View(doctor);
        }

        // GET: Doctors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", doctor.DepartmentId);
            
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DoctorId,FirstName,LastName,FullName,EmailAddress,Gender,DateOfBirth,Address,PhoneNo,Specialization,DepartmentId,ImageURL,Status")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(doctor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", doctor.DepartmentId);
            return View(doctor);
        }

        // GET: Doctors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Doctor doctor = db.Doctors.Find(id);
            db.Doctors.Remove(doctor);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //Update profile
        [Authorize(Roles = "Doctor")]
        public ActionResult UpdateProfile()
        {
            string id = User.Identity.GetUserId();
            var doctor = db.Doctors.Single(c => c.ApplicationUserId == id);
            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(Doctor model)
        {
            string id = User.Identity.GetUserId();
            var doctor = db.Doctors.Single(c => c.ApplicationUserId == id);
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
            db.SaveChanges();
            return RedirectToAction("UpdateProfile", new { id = doctor.ApplicationUserId });
        }

        [Authorize(Roles = "Doctor")]
        public ActionResult ScheduleDetail()
        {
            string user = User.Identity.GetUserId();
            var doctor = db.Doctors.Single(c => c.ApplicationUserId == user);
            var schedule = db.Schedules.Single(c => c.DoctorId == doctor.DoctorId);
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
