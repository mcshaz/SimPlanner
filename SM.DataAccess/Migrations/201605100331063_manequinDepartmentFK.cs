namespace SM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class manequinDepartmentFK : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Manequins", new[] { "DepartmentId" });
            AlterColumn("dbo.Manequins", "DepartmentId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Manequins", "DepartmentId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Manequins", new[] { "DepartmentId" });
            AlterColumn("dbo.Manequins", "DepartmentId", c => c.Guid());
            CreateIndex("dbo.Manequins", "DepartmentId");
        }
    }
}
