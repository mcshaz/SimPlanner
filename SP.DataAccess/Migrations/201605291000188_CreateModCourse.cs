namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateModCourse : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "Created", c => c.DateTime(nullable: false));
            AddColumn("dbo.Courses", "LastModified", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Courses", "LastModified");
            DropColumn("dbo.Courses", "Created");
        }
    }
}
