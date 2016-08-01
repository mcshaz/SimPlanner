namespace SP.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateModCourse : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "Created", c => c.DateTime(nullable: false /*, defaultValueSql: "GETUTCDATE()" */));
            AddColumn("dbo.Courses", "LastModified", c => c.DateTime(nullable: false /*, defaultValueSql: "GETUTCDATE()" */));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Courses", "LastModified");
            DropColumn("dbo.Courses", "Created");
        }
    }
}
