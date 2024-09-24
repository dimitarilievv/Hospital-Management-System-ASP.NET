namespace Hospital_Management_System_ASP.NET.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration17 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Prescriptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DoctorId = c.Int(nullable: false),
                        DoctorName = c.String(),
                        DoctorSpecialization = c.String(),
                        PatientId = c.Int(nullable: false),
                        UserName = c.String(),
                        PatientName = c.String(),
                        PatientGender = c.String(),
                        PatientAge = c.String(),
                        MedicalTest1 = c.String(),
                        MedicalTest2 = c.String(),
                        MedicalTest3 = c.String(),
                        MedicalTest4 = c.String(),
                        Medicine1 = c.String(),
                        Morning1 = c.Boolean(nullable: false),
                        Afternoon1 = c.Boolean(nullable: false),
                        Evening1 = c.Boolean(nullable: false),
                        Medicine2 = c.String(),
                        Morning2 = c.Boolean(nullable: false),
                        Afternoon2 = c.Boolean(nullable: false),
                        Evening2 = c.Boolean(nullable: false),
                        Medicine3 = c.String(),
                        Morning3 = c.Boolean(nullable: false),
                        Afternoon3 = c.Boolean(nullable: false),
                        Evening3 = c.Boolean(nullable: false),
                        Medicine4 = c.String(),
                        Morning4 = c.Boolean(nullable: false),
                        Afternoon4 = c.Boolean(nullable: false),
                        Evening4 = c.Boolean(nullable: false),
                        Medicine5 = c.String(),
                        Morning5 = c.Boolean(nullable: false),
                        Afternoon5 = c.Boolean(nullable: false),
                        Evening5 = c.Boolean(nullable: false),
                        Medicine6 = c.String(),
                        Morning6 = c.Boolean(nullable: false),
                        Afternoon6 = c.Boolean(nullable: false),
                        Evening6 = c.Boolean(nullable: false),
                        Medicine7 = c.String(),
                        Morning7 = c.Boolean(nullable: false),
                        Afternoon7 = c.Boolean(nullable: false),
                        Evening7 = c.Boolean(nullable: false),
                        CheckUpAfterDays = c.Int(nullable: false),
                        PrescriptionAddDate = c.DateTime(nullable: false),
                        DoctorTiming = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Doctors", t => t.DoctorId, cascadeDelete: true)
                .ForeignKey("dbo.Patients", t => t.PatientId, cascadeDelete: true)
                .Index(t => t.DoctorId)
                .Index(t => t.PatientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Prescriptions", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.Prescriptions", "DoctorId", "dbo.Doctors");
            DropIndex("dbo.Prescriptions", new[] { "PatientId" });
            DropIndex("dbo.Prescriptions", new[] { "DoctorId" });
            DropTable("dbo.Prescriptions");
        }
    }
}
