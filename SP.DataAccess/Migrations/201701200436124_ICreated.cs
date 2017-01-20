namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ICreated : DbMigration
    {
        public override void Up()
        {
            var defaultDate = new DateTime(2017, 1, 1);
            AddColumn("dbo.Departments", "CreatedUtc", c => c.DateTime(nullable: false, defaultValue: defaultDate));
            AddColumn("dbo.AspNetUsers", "CreatedUtc", c => c.DateTime(nullable: false, defaultValue: defaultDate));
            AddColumn("dbo.Institutions", "CreatedUtc", c => c.DateTime(nullable: false, defaultValue: defaultDate));
            AlterColumn("dbo.Departments", "CreatedUtc", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AspNetUsers", "CreatedUtc", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Institutions", "CreatedUtc", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Institutions", "CreatedUtc");
            DropColumn("dbo.AspNetUsers", "CreatedUtc");
            DropColumn("dbo.Departments", "CreatedUtc");
        }
    }
}
