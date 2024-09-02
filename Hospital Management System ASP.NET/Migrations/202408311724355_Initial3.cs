namespace Hospital_Management_System_ASP.NET.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Doctors", "ImageURL", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Doctors", "ImageURL");
        }
    }
}
