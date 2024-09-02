namespace Hospital_Management_System_ASP.NET.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Appointments",
                c => new
                    {
                        AppointmentId = c.Int(nullable: false, identity: true),
                        AppointmentTime = c.DateTime(),
                        Problem = c.String(),
                        Status = c.String(),
                        PatientId = c.Int(nullable: false),
                        DoctorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AppointmentId)
                .ForeignKey("dbo.Doctors", t => t.DoctorId, cascadeDelete: true)
                .ForeignKey("dbo.Patients", t => t.PatientId, cascadeDelete: true)
                .Index(t => t.PatientId)
                .Index(t => t.DoctorId);
            
            CreateTable(
                "dbo.Doctors",
                c => new
                    {
                        DoctorId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        EmailAddress = c.String(nullable: false),
                        Gender = c.String(nullable: false),
                        DateOfBirth = c.DateTime(),
                        Address = c.String(),
                        PhoneNo = c.String(nullable: false),
                        Specialization = c.String(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DoctorId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        DepartmentId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.DepartmentId);
            
            CreateTable(
                "dbo.Patients",
                c => new
                    {
                        PatientId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        DateOfBirth = c.DateTime(),
                        Gender = c.String(),
                        Address = c.String(),
                        EmailAddress = c.String(),
                        PhoneNo = c.String(),
                        BloodGroup = c.String(),
                    })
                .PrimaryKey(t => t.PatientId);
            
            CreateTable(
                "dbo.Schedules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DoctorId = c.Int(nullable: false),
                        AvailableStartDay = c.String(nullable: false),
                        AvailableEndDay = c.String(nullable: false),
                        AvailableStartTime = c.DateTime(nullable: false),
                        AvailableEndTime = c.DateTime(nullable: false),
                        TimePerPatient = c.String(nullable: false),
                        Status = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Doctors", t => t.DoctorId, cascadeDelete: true)
                .Index(t => t.DoctorId);
            
            CreateTable(
                "dbo.PatientDoctors",
                c => new
                    {
                        Patient_PatientId = c.Int(nullable: false),
                        Doctor_DoctorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Patient_PatientId, t.Doctor_DoctorId })
                .ForeignKey("dbo.Patients", t => t.Patient_PatientId, cascadeDelete: true)
                .ForeignKey("dbo.Doctors", t => t.Doctor_DoctorId, cascadeDelete: true)
                .Index(t => t.Patient_PatientId)
                .Index(t => t.Doctor_DoctorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Schedules", "DoctorId", "dbo.Doctors");
            DropForeignKey("dbo.PatientDoctors", "Doctor_DoctorId", "dbo.Doctors");
            DropForeignKey("dbo.PatientDoctors", "Patient_PatientId", "dbo.Patients");
            DropForeignKey("dbo.Appointments", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.Doctors", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Appointments", "DoctorId", "dbo.Doctors");
            DropIndex("dbo.PatientDoctors", new[] { "Doctor_DoctorId" });
            DropIndex("dbo.PatientDoctors", new[] { "Patient_PatientId" });
            DropIndex("dbo.Schedules", new[] { "DoctorId" });
            DropIndex("dbo.Doctors", new[] { "DepartmentId" });
            DropIndex("dbo.Appointments", new[] { "DoctorId" });
            DropIndex("dbo.Appointments", new[] { "PatientId" });
            DropTable("dbo.PatientDoctors");
            DropTable("dbo.Schedules");
            DropTable("dbo.Patients");
            DropTable("dbo.Departments");
            DropTable("dbo.Doctors");
            DropTable("dbo.Appointments");
        }
    }
}
