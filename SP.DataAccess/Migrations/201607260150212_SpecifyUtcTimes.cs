namespace SP.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class SpecifyUtcTimes : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.Courses", "StartTime", "StartTimeUtc");
            RenameColumn("dbo.Courses", "FinishTime", "FinishTimeUtc");
            RenameColumn("dbo.Courses", "Created", "CreatedUtc");
            RenameColumn("dbo.Courses", "LastModified", "LastModifiedUtc");
            RenameColumn("dbo.Courses", "FacultyMeetingTime", "FacultyMeetingTimeUtc");
        }

        public override void Down()
        {
            RenameColumn("dbo.Courses", "StartTimeUtc", "StartTime");
            RenameColumn("dbo.Courses", "FinishTimeUtc", "FinishTime");
            RenameColumn("dbo.Courses", "CreatedUtc", "Created");
            RenameColumn("dbo.Courses", "LastModifiedUtc", "LastModified");
            RenameColumn("dbo.Courses", "FacultyMeetingTimeUtc", "FacultyMeetingTime");
        }
    }
}
