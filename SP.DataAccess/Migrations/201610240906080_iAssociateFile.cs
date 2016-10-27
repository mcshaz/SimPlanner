namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class iAssociateFile : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CandidatePrereadings",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FileName = c.String(maxLength: 256),
                        FileModified = c.DateTime(nullable: false),
                        FileSize = c.Long(nullable: false),
                        CourseTypeId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CourseTypes", t => t.CourseTypeId, cascadeDelete: true)
                .Index(t => t.CourseTypeId);
            
            AddColumn("dbo.Institutions", "LogoImageFileName", c => c.String());
            AddColumn("dbo.Institutions", "FileModified", c => c.DateTime());
            AddColumn("dbo.Institutions", "FileSize", c => c.Long());
            AddColumn("dbo.Rooms", "FileName", c => c.String());
            AddColumn("dbo.Rooms", "FileModified", c => c.DateTime());
            AddColumn("dbo.Rooms", "FileSize", c => c.Long());
            AlterColumn("dbo.ManikinServices", "ServiceCost", c => c.Decimal(storeType: "money"));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CandidatePrereadings", "CourseTypeId", "dbo.CourseTypes");
            DropIndex("dbo.CandidatePrereadings", new[] { "CourseTypeId" });
            AlterColumn("dbo.ManikinServices", "ServiceCost", c => c.Decimal(nullable: false, storeType: "money"));
            DropColumn("dbo.Rooms", "FileSize");
            DropColumn("dbo.Rooms", "FileModified");
            DropColumn("dbo.Rooms", "FileName");
            DropColumn("dbo.Institutions", "FileSize");
            DropColumn("dbo.Institutions", "FileModified");
            DropColumn("dbo.Institutions", "LogoImageFileName");
            DropTable("dbo.CandidatePrereadings");
        }
    }
}
