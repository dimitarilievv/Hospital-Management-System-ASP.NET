namespace Hospital_Management_System_ASP.NET.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration10 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Doctors", "Gender", c => c.String());
            AlterColumn("dbo.Doctors", "PhoneNo", c => c.String());
            AlterColumn("dbo.Doctors", "Specialization", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Doctors", "Specialization", c => c.String(nullable: false));
            AlterColumn("dbo.Doctors", "PhoneNo", c => c.String(nullable: false));
            AlterColumn("dbo.Doctors", "Gender", c => c.String(nullable: false));
        }
    }
}
