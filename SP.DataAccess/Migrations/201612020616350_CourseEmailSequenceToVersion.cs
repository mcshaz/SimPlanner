namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CourseEmailSequenceToVersion : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[Courses] DROP CONSTRAINT [DF__Courses__EmailSe__286302EC]");
            RenameColumn("dbo.Courses", "EmailSequence", "Version");
            AlterColumn("dbo.Courses", "Version", c => c.Int(nullable: false));
            Sql("ALTER TABLE[dbo].[Courses] ADD  DEFAULT((0)) FOR [Version]");
        }

        public override void Down()
        {
            RenameColumn("dbo.Courses", "Version", "EmailSequence");
            AlterColumn("dbo.Courses", "EmailSequence", c => c.Byte(nullable: false));
        }
    }
}
