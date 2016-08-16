namespace SP.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class institutionAbbreviation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Institutions", "Abbreviation", c => c.String(nullable: false, maxLength: 20, defaultValue: "xxx"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Institutions", "Abbreviation");
        }
    }
}
