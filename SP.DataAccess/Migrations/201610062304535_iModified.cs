namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class iModified : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CourseSlots", "Modified", c => c.DateTime(nullable: false));
            AddColumn("dbo.Courses", "CourseDatesLastModified", c => c.DateTime(nullable: false));
            AddColumn("dbo.Courses", "FacultyMeetingDatesLastModified", c => c.DateTime(nullable: false));
            AddColumn("dbo.CourseParticipants", "EmailedUtc", c => c.DateTime());
            AddColumn("dbo.CourseSlotPresenters", "Modified", c => c.DateTime(nullable: false));
            AddColumn("dbo.CourseSlotActivities", "Modified", c => c.DateTime(nullable: false));
            AddColumn("dbo.CourseSlotManikins", "Modified", c => c.DateTime(nullable: false));
            DropColumn("dbo.Courses", "Version");
            DropColumn("dbo.Courses", "LastModifiedUtc");
            DropColumn("dbo.Courses", "EmailTimeStamp");
            DropColumn("dbo.CourseParticipants", "CreatedUtc");
            DropColumn("dbo.CourseParticipants", "LastModifiedUtc");
            DropColumn("dbo.CourseParticipants", "EmailTimeStamp");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CourseParticipants", "EmailTimeStamp", c => c.DateTime());
            AddColumn("dbo.CourseParticipants", "LastModifiedUtc", c => c.DateTime(nullable: false));
            AddColumn("dbo.CourseParticipants", "CreatedUtc", c => c.DateTime(nullable: false));
            AddColumn("dbo.Courses", "EmailTimeStamp", c => c.DateTime());
            AddColumn("dbo.Courses", "LastModifiedUtc", c => c.DateTime(nullable: false));
            AddColumn("dbo.Courses", "Version", c => c.Int(nullable: false));
            DropColumn("dbo.CourseSlotManikins", "Modified");
            DropColumn("dbo.CourseSlotActivities", "Modified");
            DropColumn("dbo.CourseSlotPresenters", "Modified");
            DropColumn("dbo.CourseParticipants", "EmailedUtc");
            DropColumn("dbo.Courses", "FacultyMeetingDatesLastModified");
            DropColumn("dbo.Courses", "CourseDatesLastModified");
            DropColumn("dbo.CourseSlots", "Modified");
        }
    }
}
