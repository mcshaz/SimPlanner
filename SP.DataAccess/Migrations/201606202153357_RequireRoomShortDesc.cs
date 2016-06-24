namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequireRoomShortDesc : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Rooms", "ShortDescription", c => c.String(nullable: false, maxLength: 32));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Rooms", "ShortDescription", c => c.String(maxLength: 32));
        }
    }
}
