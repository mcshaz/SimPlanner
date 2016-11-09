namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class institutionUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Institutions", "HomepageUrl", c => c.String(maxLength: 256));
            AlterColumn("dbo.Institutions", "StandardTimeZone", c => c.String(nullable: false, maxLength: 40));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Institutions", "StandardTimeZone", c => c.String(maxLength: 40));
            DropColumn("dbo.Institutions", "HomepageUrl");
        }
    }
}
