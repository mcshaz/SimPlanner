namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class requiredDepartmentAbbrev : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.Departments", "Colour","PrimaryColour");
            AddColumn("dbo.Departments", "SecondaryColour", c => c.String(nullable: false, maxLength: 6, fixedLength: true, defaultValue:"000000"));
            AlterColumn("dbo.Departments", "Abbreviation", c => c.String(nullable: false, maxLength: 32));
        }
        
        public override void Down()
        {
            RenameColumn("dbo.Departments", "Colour", "PrimaryColour");
            AlterColumn("dbo.Departments", "Abbreviation", c => c.String(maxLength: 16));
            DropColumn("dbo.Departments", "SecondaryColour");
        }
    }
}
