namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MealDrink : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HotDrinks",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Description = c.String(maxLength: 64),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Courses", "Cancelled", c => c.Boolean(nullable: false));
            AddColumn("dbo.CourseFormats", "HotDrinkProvided", c => c.Boolean(nullable: false));
            AddColumn("dbo.CourseFormats", "MealProvided", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "DrinkPreferenceId", c => c.Guid());
            AddColumn("dbo.AspNetUsers", "DietNotes", c => c.String(maxLength: 256));
            CreateIndex("dbo.AspNetUsers", "DrinkPreferenceId");
            AddForeignKey("dbo.AspNetUsers", "DrinkPreferenceId", "dbo.HotDrinks", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "DrinkPreferenceId", "dbo.HotDrinks");
            DropIndex("dbo.AspNetUsers", new[] { "DrinkPreferenceId" });
            DropColumn("dbo.AspNetUsers", "DietNotes");
            DropColumn("dbo.AspNetUsers", "DrinkPreferenceId");
            DropColumn("dbo.CourseFormats", "MealProvided");
            DropColumn("dbo.CourseFormats", "HotDrinkProvided");
            DropColumn("dbo.Courses", "Cancelled");
            DropTable("dbo.HotDrinks");
            //Sql(DropConstraintHelper.Drop("HotDrinks", "Unique_HotDrinkDescription"), true);
        }
    }
}
