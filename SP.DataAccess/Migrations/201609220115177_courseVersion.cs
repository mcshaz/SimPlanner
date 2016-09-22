namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class courseVersion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "Version", c => c.Int(nullable: false, defaultValue:0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Courses", "Version");
        }
    }
}
