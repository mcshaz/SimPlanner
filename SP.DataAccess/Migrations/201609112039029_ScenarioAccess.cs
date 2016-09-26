namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public partial class ScenarioAccess : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Scenarios", "Access", c => c.Int(nullable: false));
            AlterColumn("dbo.Activities", "Description", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Activities", "ResourceFilename", c => c.String(maxLength: 256));
            AlterColumn("dbo.HotDrinks", "Description", c => c.String(nullable: false, maxLength: 64));
            AlterColumn("dbo.ScenarioResources", "Description", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ScenarioResources", "ResourceFilename", c => c.String(maxLength: 256));
            foreach (var s in AddConstraints.GetConstraints().Concat(AddConstraints.GetCousinConstraint()))
            {
                Sql(s, true);
            }
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ScenarioResources", "ResourceFilename", c => c.String());
            AlterColumn("dbo.ScenarioResources", "Description", c => c.String());
            AlterColumn("dbo.HotDrinks", "Description", c => c.String(maxLength: 64));
            AlterColumn("dbo.Activities", "ResourceFilename", c => c.String());
            AlterColumn("dbo.Activities", "Description", c => c.String());
            DropColumn("dbo.Scenarios", "Access");
        }
    }
}
