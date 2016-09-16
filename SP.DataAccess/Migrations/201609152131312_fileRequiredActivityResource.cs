namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fileRequiredActivityResource : DbMigration
    {
        public override void Up()
        {
            Sql(SqlHelpers.DropConstraintIfExists("dbo.ScenarioResources", "CK_ScenarioResources_FileDetailsAllNullOrNotNull"));
            Sql(SqlHelpers.DropConstraintIfExists("dbo.ScenarioResources", "Unique_ScenarioResources_ScenarioIdFileName"));
            AddColumn("dbo.Departments", "AdminApproved", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.Institutions", "AdminApproved", c => c.Boolean(nullable: false, defaultValue: false));
            AlterColumn("dbo.ScenarioResources", "FileName", c => c.String(nullable: false, maxLength: 256));
            AlterColumn("dbo.ScenarioResources", "FileModified", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ScenarioResources", "FileSize", c => c.Long(nullable: false));

            Sql("UPDATE [dbo].[Institutions] SET AdminApproved = 1");
            Sql("UPDATE [dbo].[Departments] SET AdminApproved = 1");
            Sql(SqlHelpers.CreateUniqueConstraint<ScenarioResource>("ScenarioResources", e => e.ScenarioId, e => e.FileName));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ScenarioResources", "FileSize", c => c.Long());
            AlterColumn("dbo.ScenarioResources", "FileModified", c => c.DateTime());
            AlterColumn("dbo.ScenarioResources", "FileName", c => c.String(maxLength: 256));
            DropColumn("dbo.Institutions", "AdminApproved");
            DropColumn("dbo.Departments", "AdminApproved");
        }
    }
}
