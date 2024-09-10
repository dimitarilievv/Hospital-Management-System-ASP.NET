namespace Hospital_Management_System_ASP.NET.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration13 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Departments", "Status", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Departments", "Status");
        }
    }
}
