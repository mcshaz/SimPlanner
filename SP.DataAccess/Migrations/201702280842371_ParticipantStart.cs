namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ParticipantStart : DbMigration
    {

        public override void Up()
        {
            AddColumn("dbo.CourseSlots", "FacultyOnly", c => c.Boolean(nullable: false));

            AddColumn("dbo.Courses", "DurationParticipantMins", c => c.Int(nullable: false));
            AddColumn("dbo.Courses", "StartParticipantUtc", c => c.DateTime(nullable: false));

            AddColumn("dbo.CourseDays", "DurationParticipantMins", c => c.Int(nullable: false));
            AddColumn("dbo.CourseDays", "StartParticipantUtc", c => c.DateTime(nullable: false));

            RenameColumn("dbo.Courses", "DurationMins", "DurationFacultyMins");
            RenameColumn("dbo.Courses", "StartUtc", "StartFacultyUtc");

            RenameColumn("dbo.CourseDays", "StartUtc", "StartFacultyUtc");
            RenameColumn("dbo.CourseDays", "DurationMins", "DurationFacultyMins");

        }

        public override void Down()
        {
            DropColumn("dbo.CourseSlots", "FacultyOnly");

            DropColumn("dbo.Courses", "DurationParticipantMins");
            DropColumn("dbo.Courses", "StartParticipantUtc");

            DropColumn("dbo.CourseDays", "DurationParticipantMins");
            DropColumn("dbo.CourseDays", "StartParticipantUtc");

            RenameColumn("dbo.Courses", "DurationFacultyMins", "DurationMins");
            RenameColumn("dbo.Courses", "StartFacultyUtc", "StartUtc");

            RenameColumn("dbo.CourseDays", "StartFacultyUtc", "StartUtc");
            RenameColumn("dbo.CourseDays", "DurationFacultyMins", "DurationMins");
        }
    }
}
