namespace SP.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class prereadingFilenameNonNul : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CandidatePrereadings", "FileName", c => c.String(nullable: false, maxLength: 256));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CandidatePrereadings", "FileName", c => c.String(maxLength: 256));
        }
    }
}
