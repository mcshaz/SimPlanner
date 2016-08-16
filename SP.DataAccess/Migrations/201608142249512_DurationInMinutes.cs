namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DurationInMinutes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "DurationMins", c => c.Int(nullable: false));
            AddColumn("dbo.CourseDays", "DurationMins", c => c.Int(nullable: false));
            DropColumn("dbo.Courses", "Duration");
            DropColumn("dbo.CourseDays", "Duration");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CourseDays", "Duration", c => c.Time(nullable: false, precision: 7));
            AddColumn("dbo.Courses", "Duration", c => c.Time(nullable: false, precision: 7));
            DropColumn("dbo.CourseDays", "DurationMins");
            DropColumn("dbo.Courses", "DurationMins");
        }
    }
}
