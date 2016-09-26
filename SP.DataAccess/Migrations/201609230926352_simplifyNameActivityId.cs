namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class simplifyNameActivityId : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ActivityTeachingResources", newName: "Activities");
            RenameColumn(table: "dbo.CourseSlotActivities", name: "ActivityTeachingResourceId", newName: "ActivityId");
            RenameIndex(table: "dbo.CourseSlotActivities", name: "IX_ActivityTeachingResourceId", newName: "IX_ActivityId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.CourseSlotActivities", name: "IX_ActivityId", newName: "IX_ActivityTeachingResourceId");
            RenameColumn(table: "dbo.CourseSlotActivities", name: "ActivityId", newName: "ActivityTeachingResourceId");
            RenameTable(name: "dbo.Activities", newName: "ActivityTeachingResources");
        }
    }
}
