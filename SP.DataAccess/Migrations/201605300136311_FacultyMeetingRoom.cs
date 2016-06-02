namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FacultyMeetingRoom : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "FacultyMeetingRoomId", c => c.Guid());
            AddColumn("dbo.Courses", "FacultyMeetingDuration", c => c.Int());
            CreateIndex("dbo.Courses", "FacultyMeetingRoomId");
            AddForeignKey("dbo.Courses", "FacultyMeetingRoomId", "dbo.Rooms", "Id");
            Sql("update __MigrationHistory set ContextKey = 'SP.DataAccess.MedSimDbContext'");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Courses", "FacultyMeetingRoomId", "dbo.Rooms");
            DropIndex("dbo.Courses", new[] { "FacultyMeetingRoomId" });
            DropColumn("dbo.Courses", "FacultyMeetingDuration");
            DropColumn("dbo.Courses", "FacultyMeetingRoomId");
        }
    }
}
