namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mergeCourseSlotActivityScenario : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.CourseSlotScenarios", newName: "CourseSlotActivities");
            DropForeignKey("dbo.ChosenTeachingResources", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.ChosenTeachingResources", "Participant_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ChosenTeachingResources", "CourseSlotId", "dbo.CourseSlots");
            DropForeignKey("dbo.ChosenTeachingResources", "ActivityId", "dbo.Activities");
            DropIndex("dbo.ChosenTeachingResources", new[] { "CourseId" });
            DropIndex("dbo.ChosenTeachingResources", new[] { "CourseSlotId" });
            DropIndex("dbo.ChosenTeachingResources", new[] { "ActivityId" });
            DropIndex("dbo.ChosenTeachingResources", new[] { "Participant_Id" });
            DropIndex("dbo.CourseSlotActivities", new[] { "ScenarioId" });
            DropPrimaryKey("dbo.CourseSlotActivities");
            AddColumn("dbo.CourseSlotActivities", "ActivityId", c => c.Guid());
            AlterColumn("dbo.CourseSlotActivities", "ScenarioId", c => c.Guid());
            AddPrimaryKey("dbo.CourseSlotActivities", new[] { "CourseId", "CourseSlotId", "StreamNumber" });
            CreateIndex("dbo.CourseSlotActivities", "ScenarioId");
            CreateIndex("dbo.CourseSlotActivities", "ActivityId");
            AddForeignKey("dbo.CourseSlotActivities", "ActivityId", "dbo.Activities", "Id");
            DropTable("dbo.ChosenTeachingResources");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ChosenTeachingResources",
                c => new
                    {
                        CourseId = c.Guid(nullable: false),
                        CourseSlotId = c.Guid(nullable: false),
                        ActivityId = c.Guid(nullable: false),
                        Participant_Id = c.Guid(),
                    })
                .PrimaryKey(t => new { t.CourseId, t.CourseSlotId, t.ActivityId });
            
            DropForeignKey("dbo.CourseSlotActivities", "ActivityId", "dbo.Activities");
            DropIndex("dbo.CourseSlotActivities", new[] { "ActivityId" });
            DropIndex("dbo.CourseSlotActivities", new[] { "ScenarioId" });
            DropPrimaryKey("dbo.CourseSlotActivities");
            AlterColumn("dbo.CourseSlotActivities", "ScenarioId", c => c.Guid(nullable: false));
            DropColumn("dbo.CourseSlotActivities", "ActivityId");
            AddPrimaryKey("dbo.CourseSlotActivities", new[] { "CourseId", "CourseSlotId" });
            CreateIndex("dbo.CourseSlotActivities", "ScenarioId");
            CreateIndex("dbo.ChosenTeachingResources", "Participant_Id");
            CreateIndex("dbo.ChosenTeachingResources", "ActivityId");
            CreateIndex("dbo.ChosenTeachingResources", "CourseSlotId");
            CreateIndex("dbo.ChosenTeachingResources", "CourseId");
            AddForeignKey("dbo.ChosenTeachingResources", "ActivityId", "dbo.Activities", "Id");
            AddForeignKey("dbo.ChosenTeachingResources", "CourseSlotId", "dbo.CourseSlots", "Id");
            AddForeignKey("dbo.ChosenTeachingResources", "Participant_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.ChosenTeachingResources", "CourseId", "dbo.Courses", "Id");
            RenameTable(name: "dbo.CourseSlotActivities", newName: "CourseSlotScenarios");
        }
    }
}
