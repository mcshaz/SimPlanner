namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class manequinDepartmentFK : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Manikins", new[] { "DepartmentId" });
            AlterColumn("dbo.Manikins", "DepartmentId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Manikins", "DepartmentId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Manikins", new[] { "DepartmentId" });
            AlterColumn("dbo.Manikins", "DepartmentId", c => c.Guid());
            CreateIndex("dbo.Manikins", "DepartmentId");
        }
    }
}
