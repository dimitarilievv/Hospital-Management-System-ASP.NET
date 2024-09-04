namespace Hospital_Management_System_ASP.NET.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Doctors", "Status", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Doctors", "Status");
        }
    }
}
