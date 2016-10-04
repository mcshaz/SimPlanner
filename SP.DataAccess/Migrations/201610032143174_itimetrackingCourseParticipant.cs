namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class itimetrackingCourseParticipant : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "EmailTimeStamp", c => c.DateTime());
            AddColumn("dbo.CourseParticipants", "CreatedUtc", c => c.DateTime(nullable: false));
            AddColumn("dbo.CourseParticipants", "LastModifiedUtc", c => c.DateTime(nullable: false));
            AddColumn("dbo.CourseParticipants", "EmailTimeStamp", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CourseParticipants", "EmailTimeStamp");
            DropColumn("dbo.CourseParticipants", "LastModifiedUtc");
            DropColumn("dbo.CourseParticipants", "CreatedUtc");
            DropColumn("dbo.Courses", "EmailTimeStamp");
        }
    }
}
