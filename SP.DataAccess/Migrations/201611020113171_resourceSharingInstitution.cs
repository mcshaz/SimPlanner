namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class resourceSharingInstitution : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProfessionalRoleInstitutions", "InstitutionId", "dbo.Institutions");
            CreateTable(
                "dbo.ResourceSharingInstitutions",
                c => new
                    {
                        Institution1Id = c.Guid(nullable: false),
                        Institution2Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Institution1Id, t.Institution2Id })
                .ForeignKey("dbo.Institutions", t => t.Institution1Id)
                .ForeignKey("dbo.Institutions", t => t.Institution2Id)
                .Index(t => t.Institution1Id)
                .Index(t => t.Institution2Id);
            
            AddForeignKey("dbo.ProfessionalRoleInstitutions", "InstitutionId", "dbo.Institutions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProfessionalRoleInstitutions", "InstitutionId", "dbo.Institutions");
            DropForeignKey("dbo.ResourceSharingInstitutions", "Institution2Id", "dbo.Institutions");
            DropForeignKey("dbo.ResourceSharingInstitutions", "Institution1Id", "dbo.Institutions");
            DropIndex("dbo.ResourceSharingInstitutions", new[] { "Institution2Id" });
            DropIndex("dbo.ResourceSharingInstitutions", new[] { "Institution1Id" });
            DropTable("dbo.ResourceSharingInstitutions");
            AddForeignKey("dbo.ProfessionalRoleInstitutions", "InstitutionId", "dbo.Institutions", "Id", cascadeDelete: true);
        }
    }
}
