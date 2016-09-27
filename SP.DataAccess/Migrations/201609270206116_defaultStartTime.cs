namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class defaultStartTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CourseFormats", "DefaultStartTime", c => c.Time(nullable: false, precision: 7, defaultValue: TimeSpan.FromHours(8)));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CourseFormats", "DefaultStartTime");
        }
    }
}
