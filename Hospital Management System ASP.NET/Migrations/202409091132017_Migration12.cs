namespace Hospital_Management_System_ASP.NET.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patients", "FullName", c => c.String());
            AlterColumn("dbo.Appointments", "Status", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Appointments", "Status", c => c.String());
            DropColumn("dbo.Patients", "FullName");
        }
    }
}
