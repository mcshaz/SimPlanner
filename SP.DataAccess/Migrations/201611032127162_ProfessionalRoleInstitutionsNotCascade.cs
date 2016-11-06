namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProfessionalRoleInstitutionsNotCascade : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProfessionalRoleInstitutions", "InstitutionId", "dbo.Institutions");
            AddForeignKey("dbo.ProfessionalRoleInstitutions", "InstitutionId", "dbo.Institutions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProfessionalRoleInstitutions", "InstitutionId", "dbo.Institutions");
            AddForeignKey("dbo.ProfessionalRoleInstitutions", "InstitutionId", "dbo.Institutions", "Id", cascadeDelete: true);
        }
    }
}
