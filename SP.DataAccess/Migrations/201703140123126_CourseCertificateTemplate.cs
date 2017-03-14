namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CourseCertificateTemplate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CourseTypes", "CertificateFileName", c => c.String());
            AddColumn("dbo.CourseTypes", "FileModified", c => c.DateTime());
            AddColumn("dbo.CourseTypes", "FileSize", c => c.Long());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CourseTypes", "FileSize");
            DropColumn("dbo.CourseTypes", "FileModified");
            DropColumn("dbo.CourseTypes", "CertificateFileName");
        }
    }
}
