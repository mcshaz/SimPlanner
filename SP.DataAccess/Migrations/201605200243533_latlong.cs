namespace SP.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class latlong : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Institutions", "Latitude", c => c.Double());
            AddColumn("dbo.Institutions", "Longitude", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Institutions", "Longitude");
            DropColumn("dbo.Institutions", "Latitude");
        }
    }
}
