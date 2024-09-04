namespace Hospital_Management_System_ASP.NET.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Doctors", "ApplicationUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.Patients", "ApplicationUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Doctors", "ApplicationUserId");
            CreateIndex("dbo.Patients", "ApplicationUserId");
            AddForeignKey("dbo.Doctors", "ApplicationUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Patients", "ApplicationUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Patients", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Doctors", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Patients", new[] { "ApplicationUserId" });
            DropIndex("dbo.Doctors", new[] { "ApplicationUserId" });
            DropColumn("dbo.Patients", "ApplicationUserId");
            DropColumn("dbo.Doctors", "ApplicationUserId");
        }
    }
}
