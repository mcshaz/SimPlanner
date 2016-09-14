namespace SP.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class ResourceFileDetails : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[ActivityTeachingResources] DROP CONSTRAINT [CheckCousinsFilenames]");
            Sql("DROP FUNCTION [dbo].[OtherTeachingResourcesWithSameFilename]");
            Sql("DROP INDEX [Unique_ScenarioResources_ScenarioIdResourceFilename] ON [dbo].[ScenarioResources]");

            AddColumn("dbo.ActivityTeachingResources", "FileName", c => c.String(maxLength: 256));
            AddColumn("dbo.ActivityTeachingResources", "FileModified", c => c.DateTime());
            AddColumn("dbo.ActivityTeachingResources", "FileSize", c => c.Long());
            AddColumn("dbo.ScenarioResources", "FileName", c => c.String(maxLength: 256));
            AddColumn("dbo.ScenarioResources", "FileModified", c => c.DateTime());
            AddColumn("dbo.ScenarioResources", "FileSize", c => c.Long());
            DropColumn("dbo.ActivityTeachingResources", "ResourceFilename");
            DropColumn("dbo.Scenarios", "TemplateFilename");
            DropColumn("dbo.ScenarioResources", "ResourceFilename");

            foreach (var tbl in new[] { "ScenarioResources", "ActivityTeachingResources" })
            {
                string constraintName = $"CK_{tbl}_FileDetailsAllNullOrNotNull";
                Sql(string.Format(@"ALTER TABLE dbo.{0} WITH CHECK ADD CONSTRAINT {1} CHECK(([FileName] IS NULL AND [FileModified] IS NULL AND [FileSize] IS NULL) OR ([FileName] IS  NOT NULL AND [FileModified] IS NOT NULL AND [FileSize] IS NOT NULL));
ALTER TABLE dbo.{0} CHECK CONSTRAINT {1};", tbl, constraintName), true);
            }

            Sql(SqlHelpers.CreateUniqueConstraint<ScenarioResource>("ScenarioResources", e => e.ScenarioId, e => e.FileName));

            Sql(@"CREATE FUNCTION [dbo].[OtherTeachingResourcesWithSameFilename]
(
	-- Add the parameters for the function here
	@courseActivityId uniqueidentifier,
	@activityTeachingResourceId uniqueidentifier,
	@filename nvarchar
)
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @ResultVar bit

	-- Add the T-SQL statements to compute the return value here
	SET @ResultVar = CASE 
		WHEN @filename IS NOT NULL AND EXISTS(
			SELECT 1
			FROM dbo.CourseActivities as allCa
			INNER JOIN dbo.ActivityTeachingResources as atr ON atr.CourseActivityId = allCa.Id
			WHERE allCa.CourseTypeId IN 
				(SELECT ca.CourseTypeId
				 FROM dbo.CourseActivities as ca
				 WHERE ca.Id = @CourseActivityId)
			AND atr.FileName = @filename AND atr.Id <> @activityTeachingResourceId
		)
		THEN 1
		ELSE 0
	END

	-- Return the result of the function
	RETURN @ResultVar

END;");
            Sql(@"ALTER TABLE[dbo].[ActivityTeachingResources]  WITH CHECK ADD  CONSTRAINT[CheckCousinsFilenames] CHECK(([dbo].[OtherTeachingResourcesWithSameFilename]([CourseActivityId],[Id],[FileName]) = (0)));
ALTER TABLE [dbo].[ActivityTeachingResources] CHECK CONSTRAINT [CheckCousinsFilenames];");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ScenarioResources", "ResourceFilename", c => c.String(maxLength: 256));
            AddColumn("dbo.Scenarios", "TemplateFilename", c => c.String(maxLength: 256));
            AddColumn("dbo.ActivityTeachingResources", "ResourceFilename", c => c.String(maxLength: 256));
            DropColumn("dbo.ScenarioResources", "FileSize");
            DropColumn("dbo.ScenarioResources", "FileModified");
            DropColumn("dbo.ScenarioResources", "FileName");
            DropColumn("dbo.ActivityTeachingResources", "FileSize");
            DropColumn("dbo.ActivityTeachingResources", "FileModified");
            DropColumn("dbo.ActivityTeachingResources", "FileName");
        }
    }
}
