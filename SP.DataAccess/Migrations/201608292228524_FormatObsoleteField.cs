namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FormatObsoleteField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CourseFormats", "Obsolete", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CourseFormats", "Obsolete");
        }
    }
}
