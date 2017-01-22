namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmailScheduling : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CourseHangfireJobs",
                c => new
                    {
                        CourseId = c.Guid(nullable: false),
                        HangfireId = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => new { t.CourseId, t.HangfireId })
                .ForeignKey("dbo.Courses", t => t.CourseId)
                //.ForeignKey("HangFire.Server", t=>t.HangfireId)
                .Index(t => t.CourseId);

            //Sql("CREATE INDEX [IX_HangfireId] ON [dbo].[CourseHangfireJobs]([HangfireId])");
            //Sql("ALTER TABLE [dbo].[CourseHangfireJobs] ADD CONSTRAINT[FK_dbo.CourseHangfireJobs_HangFire.Server_HangfireId] FOREIGN KEY([HangfireId]) REFERENCES [HangFire].[Server]([Id])");

            AddColumn("dbo.CourseTypes", "SendCandidateTimetable", c => c.Boolean(nullable: false));
            AddColumn("dbo.CandidatePrereadings", "SendRelativeToCourse", c => c.Short());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CourseHangfireJobs", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.CourseHangfireJobs", "HangfireId", "HangFire.[Server]");
            DropIndex("dbo.CourseHangfireJobs", new[] { "CourseId" });
            DropColumn("dbo.CandidatePrereadings", "SendRelativeToCourse");
            DropColumn("dbo.CourseTypes", "SendCandidateTimetable");
            DropTable("dbo.CourseHangfireJobs");
        }
    }
}
