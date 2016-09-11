namespace SP.DataAccess.Migrations
{
    using Enums;
    using System.Data.Entity.Migrations;

    public partial class ScenarioAccess : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Scenarios", "Access", c => c.Int(nullable: false, defaultValue: (int)SharingLevel.DepartmentOnly));
            Sql(DropConstraintHelper.DropIfExists("ProfessionalRoles", "Unique_RoleDescription"), true);
            Sql("ALTER TABLE dbo.ProfessionalRoles ADD CONSTRAINT Unique_RoleDescription UNIQUE([Category],[Description])", true);
            Sql("ALTER TABLE dbo.HotDrinks ADD CONSTRAINT Unique_HotDrinkDescription UNIQUE([Description])", true);
        }
        
        public override void Down()
        {
            DropColumn("dbo.Scenarios", "Access");
            Sql(DropConstraintHelper.DropIfExists("ProfessionalRoles", "Unique_RoleDescription"), true);
            Sql(DropConstraintHelper.DropIfExists("HotDrinks", "Unique_HotDrinkDescription"), true);
        }
    }
}
