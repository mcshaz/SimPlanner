namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IAdminApproved : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "AdminApproved", c => c.Boolean(nullable: false));
            Sql("UPDATE dbo.AspNetUsers SET AdminApproved=1");
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "AdminApproved");
        }
    }
}
