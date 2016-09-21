namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameManikin : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Manequins", newName: "Manikins");
            RenameTable(name: "dbo.CourseSlotManequins", newName: "CourseSlotManikins");
            RenameTable(name: "dbo.ManequinServices", newName: "ManikinServices");
            RenameTable(name: "dbo.ManequinModels", newName: "ManikinModels");
            RenameTable(name: "dbo.ManequinManufacturers", newName: "ManikinManufacturers");
            RenameColumn("dbo.ManikinServices", "ManequinId", "ManikinId");
            RenameColumn("dbo.CourseSlotManikins", "ManequinId", "ManikinId");

            Sql(@"DECLARE @sql1 NVARCHAR(max)=N''
                  SELECT @sql1 += Char(10) + N'EXEC sp_rename N''' 
                     + t.n
                     + ''', N''' + REPLACE(t.n,'Manequin','Manikin')
                     + ''', 'N'OBJECT'';' 
			      FROM (
                            SELECT QuoteName(CONSTRAINT_SCHEMA)+'.'+QuoteName(CONSTRAINT_NAME) as n 
                            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                            WHERE CHARINDEX('Manequin', CONSTRAINT_NAME,0)>0
                     ) as t; 
                     EXEC sp_executesql @sql1;");

            Sql(@"DECLARE @sql2 NVARCHAR(max)=N''
                    SELECT @sql2 += Char(10) + N'EXEC sp_rename N''' 
                        + QuoteName(i.[Schema]) + '.' + QuoteName(i.[table_name]) + '.' + i.[index_name]
                        + ''', N''' + REPLACE(i.[index_name],'Manequin','Manikin')
                        + ''', N''INDEX'';'
                    FROM (
		                    SELECT OBJECT_SCHEMA_NAME(T.[object_id],DB_ID()) AS [Schema],  
			                    T.[name] AS [table_name], QuoteName(I.[name]) AS [index_name]
		                    FROM sys.[tables] AS T  
			                    INNER JOIN sys.[indexes] I ON T.[object_id] = I.[object_id]  
		                    WHERE LEFT(I.[name],3) = N'IX_' AND CHARINDEX('Manequin', I.[name],0)>0
                        ) as i; 
                    EXEC sp_executesql @sql2; ");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ManequinManufacturers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ManequinModels",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Description = c.String(nullable: false, maxLength: 128),
                        ManufacturerId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ManequinServices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProblemDescription = c.String(maxLength: 1028),
                        ServicedInternally = c.Boolean(nullable: false),
                        Sent = c.DateTime(nullable: false, storeType: "date"),
                        Returned = c.DateTime(storeType: "date"),
                        PriceEstimate = c.Decimal(storeType: "money"),
                        ServiceCost = c.Decimal(nullable: false, storeType: "money"),
                        ManequinId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CourseSlotManequins",
                c => new
                    {
                        CourseId = c.Guid(nullable: false),
                        CourseSlotId = c.Guid(nullable: false),
                        ManequinId = c.Guid(nullable: false),
                        StreamNumber = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => new { t.CourseId, t.CourseSlotId, t.ManequinId });
            
            DropForeignKey("dbo.CourseSlotManikins", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.CourseSlotManikins", "CourseSlotId", "dbo.CourseSlots");
            DropForeignKey("dbo.ManikinModels", "ManufacturerId", "dbo.ManikinManufacturers");
            DropForeignKey("dbo.ManikinServices", "ManikinId", "dbo.Manikins");
            DropForeignKey("dbo.CourseSlotManikins", "ManikinId", "dbo.Manikins");
            DropIndex("dbo.ManikinModels", new[] { "ManufacturerId" });
            DropIndex("dbo.ManikinServices", new[] { "ManikinId" });
            DropIndex("dbo.CourseSlotManikins", new[] { "ManikinId" });
            DropIndex("dbo.CourseSlotManikins", new[] { "CourseSlotId" });
            DropIndex("dbo.CourseSlotManikins", new[] { "CourseId" });
            DropTable("dbo.ManikinManufacturers");
            DropTable("dbo.ManikinModels");
            DropTable("dbo.ManikinServices");
            DropTable("dbo.CourseSlotManikins");
            CreateIndex("dbo.ManequinModels", "ManufacturerId");
            CreateIndex("dbo.ManequinServices", "ManequinId");
            CreateIndex("dbo.CourseSlotManequins", "ManequinId");
            CreateIndex("dbo.CourseSlotManequins", "CourseSlotId");
            CreateIndex("dbo.CourseSlotManequins", "CourseId");
            AddForeignKey("dbo.CourseSlotManequins", "CourseId", "dbo.Courses", "Id");
            AddForeignKey("dbo.CourseSlotManequins", "CourseSlotId", "dbo.CourseSlots", "Id");
            AddForeignKey("dbo.ManequinModels", "ManufacturerId", "dbo.ManequinManufacturers", "Id");
            AddForeignKey("dbo.ManequinServices", "ManequinId", "dbo.Manequins", "Id");
            AddForeignKey("dbo.CourseSlotManequins", "ManequinId", "dbo.Manequins", "Id");
            RenameTable(name: "dbo.Manikins", newName: "Manequins");
        }
    }
}
