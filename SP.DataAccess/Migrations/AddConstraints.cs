using System.Collections.Generic;

namespace SP.DataAccess.Migrations
{
    class AddConstraints
    {
        internal static IEnumerable<string> GetConstraints()
        {
            return new[] {
                SqlHelpers.DropConstraintIfExists("ProfessionalRoles", "Unique_RoleDescription"),
                SqlHelpers.CreateUniqueConstraint<ProfessionalRole>("ProfessionalRoles", e => e.Category, e => e.Description),
                SqlHelpers.CreateUniqueConstraint<HotDrink>("HotDrinks", e => e.Description),

                SqlHelpers.CreateUniqueConstraint<Institution>("Institutions", e => e.LocaleCode, e => e.Name),
                SqlHelpers.CreateUniqueConstraint<Department>("Departments", e => e.InstitutionId, e => e.Name),
                SqlHelpers.CreateUniqueConstraint<Department>("Departments", e => e.InstitutionId, e => e.Abbreviation),
                SqlHelpers.CreateUniqueConstraint<Scenario>("Scenarios", e => e.DepartmentId, e => e.BriefDescription),
                SqlHelpers.CreateUniqueConstraint<Manikin>("Manikins", e => e.DepartmentId, e => e.Description),

                SqlHelpers.CreateUniqueConstraint<ScenarioResource>("ScenarioResources", e => e.ScenarioId, e => e.FileName)
            };
        }
        internal static string[] GetCousinConstraint()
        {
            return new[] {
                @"CREATE FUNCTION OtherTeachingResourcesWithSameFilename
(
	-- Add the parameters for the function here
	@courseActivityId uniqueidentifier,
	@activityId uniqueidentifier,
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
			AND atr.ResourceFilename = @filename AND atr.Id <> @activityId
		)
		THEN 1
		ELSE 0
	END

	-- Return the result of the function
	RETURN @ResultVar

END;",
                @"ALTER TABLE [dbo].[Activities]  WITH CHECK ADD  CONSTRAINT [CheckCousinsFilenames] CHECK  (([dbo].[OtherTeachingResourcesWithSameFilename]([CourseActivityId],[Id],[ResourceFileName])=(0)));",
                @"ALTER TABLE [dbo].[Activities] CHECK CONSTRAINT [CheckCousinsFilenames];"
            };
        }
    }
}
