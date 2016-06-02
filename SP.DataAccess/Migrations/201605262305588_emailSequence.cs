namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class emailSequence : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "EmailSequence", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Courses", "EmailSequence");
        }
    }
}
