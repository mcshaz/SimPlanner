namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class trackParticipants : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CourseSlots", "TrackParticipants", c => c.Boolean(nullable: false, defaultValue:true));
            Sql(@"
ALTER TABLE [dbo].[Activities] DROP CONSTRAINT [CheckCousinsFilenames]
GO
DROP FUNCTION [dbo].[OtherTeachingResourcesWithSameFilename]
GO

CREATE FUNCTION [dbo].[OtherTeachingResourcesWithSameFilename]
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
			INNER JOIN dbo.Activities as atr ON atr.CourseActivityId = allCa.Id
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

END;
GO

ALTER TABLE [dbo].[Activities]  WITH CHECK ADD  CONSTRAINT [CheckCousinsFilenames] CHECK  (([dbo].[OtherTeachingResourcesWithSameFilename]([CourseActivityId],[Id],[FileName])=(0)))
GO

ALTER TABLE [dbo].[Activities] CHECK CONSTRAINT [CheckCousinsFilenames]
GO
");
        }
        
        public override void Down()
        {
            DropColumn("dbo.CourseSlots", "TrackParticipants");
        }
    }
}
