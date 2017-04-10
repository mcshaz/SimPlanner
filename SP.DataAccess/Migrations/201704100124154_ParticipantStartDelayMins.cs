namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ParticipantStartDelayMins : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "DelayStartParticipantMins", c => c.Int(nullable: false, defaultValue: 0));
            Sql("UPDATE dbo.Courses SET DelayStartParticipantMins = datediff(mi, StartFacultyUtc, StartParticipantUtc) WHERE StartParticipantUtc <> {ts'1900-01-01 00:00:00.000'}");
            Sql("UPDATE dbo.Courses SET DelayStartParticipantMins = 0 WHERE DelayStartParticipantMins < 0");
            DropColumn("dbo.Courses", "StartParticipantUtc");
            
            AddColumn("dbo.CourseDays", "DelayStartParticipantMins", c => c.Int(nullable: false, defaultValue: 0));
            Sql("UPDATE dbo.CourseDays SET DelayStartParticipantMins = datediff(mi, StartFacultyUtc, StartParticipantUtc) WHERE StartParticipantUtc <> {ts'1900-01-01 00:00:00.000'}");
            DropColumn("dbo.CourseDays", "StartParticipantUtc");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CourseDays", "StartParticipantUtc", c => c.DateTime(nullable: false));
            AddColumn("dbo.Courses", "StartParticipantUtc", c => c.DateTime(nullable: false));
            DropColumn("dbo.CourseDays", "DelayStartParticipantMins");
            DropColumn("dbo.Courses", "DelayStartParticipantMins");
        }
    }
}
