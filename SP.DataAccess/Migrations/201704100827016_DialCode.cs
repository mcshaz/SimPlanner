namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DialCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cultures", "DialCode", c => c.String(maxLength: 7));
            Sql("Update dbo.Cultures set DialCode = CASE CountryCode WHEN 840 THEN 1 ELSE CountryCode END");
            Sql("Update dbo.Cultures set Name = 'United States' WHERE CountryCode = 840");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cultures", "DialCode");
        }
    }
}
