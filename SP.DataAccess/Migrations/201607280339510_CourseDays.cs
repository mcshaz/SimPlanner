namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CourseDays : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CourseDays",
                c => new
                    {
                        CourseId = c.Guid(nullable: false),
                        Day = c.Int(nullable: false),
                        StartUtc = c.DateTime(nullable: false),
                        Duration = c.Time(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => new { t.CourseId, t.Day })
                .ForeignKey("dbo.Courses", t => t.CourseId)
                .Index(t => t.CourseId);

            RenameColumn("dbo.Courses", "StartTimeUtc", "StartUtc");
            RenameColumn("dbo.Courses", "FacultyMeetingTimeUtc", "FacultyMeetingUtc");
            DropColumn("dbo.Courses", "FinishTimeUtc");
            AddColumn("dbo.Courses", "Duration", c => c.Time(nullable: false, precision: 7));
        }
        
        public override void Down()
        {

            RenameColumn("dbo.Courses",  "StartUtc", "StartTimeUtc");
            RenameColumn("dbo.Courses", "FacultyMeetingUtc", "FacultyMeetingTimeUtc");
            AddColumn("dbo.Courses", "FinishTimeUtc", c=>c.DateTime(nullable:false));
            DropColumn("dbo.Courses", "Duration");

            DropForeignKey("dbo.CourseDays", "CourseId", "dbo.Courses");
            DropIndex("dbo.CourseDays", new[] { "CourseId" });
            DropTable("dbo.CourseDays");
        }
    }
}
