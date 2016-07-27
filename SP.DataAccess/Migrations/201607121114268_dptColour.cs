namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dptColour : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Departments", "Colour", c => c.String(nullable: false, maxLength: 6, fixedLength: true, defaultValueSql:"'000000'"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Departments", "Colour");
        }
    }
}
