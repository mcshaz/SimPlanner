namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CourseFacultyInvites : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CourseFacultyInvites",
                c => new
                    {
                        CourseId = c.Guid(nullable: false),
                        ParticipantId = c.Guid(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.CourseId, t.ParticipantId })
                .ForeignKey("dbo.AspNetUsers", t => t.ParticipantId)
                .ForeignKey("dbo.Courses", t => t.CourseId)
                .Index(t => t.CourseId)
                .Index(t => t.ParticipantId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CourseFacultyInvites", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.CourseFacultyInvites", "ParticipantId", "dbo.AspNetUsers");
            DropIndex("dbo.CourseFacultyInvites", new[] { "ParticipantId" });
            DropIndex("dbo.CourseFacultyInvites", new[] { "CourseId" });
            DropTable("dbo.CourseFacultyInvites");
        }
    }
}
