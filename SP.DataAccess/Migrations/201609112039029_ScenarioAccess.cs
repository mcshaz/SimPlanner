namespace SP.DataAccess.Migrations
{
    using Enums;
    using System.Data.Entity.Migrations;

    public partial class ScenarioAccess : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Scenarios", "Access", c => c.Int(nullable: false, defaultValue: (int)SharingLevel.DepartmentOnly));
            Sql(SqlHelpers.DropConstraintIfExists("ProfessionalRoles", "Unique_RoleDescription"), true);
            Sql(SqlHelpers.CreateUniqueConstraint<ProfessionalRole>("ProfessionalRoles", e => e.Category, e=>e.Description),true);
            Sql(SqlHelpers.CreateUniqueConstraint<HotDrink>("HotDrinks", e=>e.Description), true);

            Sql(SqlHelpers.CreateUniqueConstraint<Institution>("Institutions", e => e.LocaleCode, e => e.Name), true);
            Sql(SqlHelpers.CreateUniqueConstraint<Department>("Departments", e => e.InstitutionId, e => e.Name), true);
            Sql(SqlHelpers.CreateUniqueConstraint<Department>("Departments", e => e.InstitutionId, e => e.Abbreviation), true);
            Sql(SqlHelpers.CreateUniqueConstraint<Scenario>("Scenarios", e => e.DepartmentId, e => e.BriefDescription), true);
            Sql(SqlHelpers.CreateUniqueConstraint<Manequin>("Manequins", e => e.DepartmentId, e => e.Description), true);

            Sql(SqlHelpers.CreateUniqueConstraint<ScenarioResource>("ScenarioResources", e => e.ScenarioId, e => e.ResourceFilename), true);
        }
        
        public override void Down()
        {
            DropColumn("dbo.Scenarios", "Access");
            Sql(SqlHelpers.DropConstraintIfExists("ProfessionalRoles", "Unique_RoleDescription"), true);
            Sql(SqlHelpers.DropConstraintIfExists("HotDrinks", "Unique_HotDrinkDescription"), true);
        }
    }
}
