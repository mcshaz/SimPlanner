namespace SP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activities",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CourseActivityId = c.Guid(nullable: false),
                        Description = c.String(),
                        ResourceFilename = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CourseActivities", t => t.CourseActivityId)
                .Index(t => t.CourseActivityId);
            
            CreateTable(
                "dbo.ChosenTeachingResources",
                c => new
                    {
                        CourseId = c.Guid(nullable: false),
                        CourseSlotId = c.Guid(nullable: false),
                        ActivityId = c.Guid(nullable: false),
                        Participant_Id = c.Guid(),
                    })
                .PrimaryKey(t => new { t.CourseId, t.CourseSlotId, t.ActivityId })
                .ForeignKey("dbo.Courses", t => t.CourseId)
                .ForeignKey("dbo.AspNetUsers", t => t.Participant_Id)
                .ForeignKey("dbo.CourseSlots", t => t.CourseSlotId)
                .ForeignKey("dbo.Activities", t => t.ActivityId)
                .Index(t => t.CourseId)
                .Index(t => t.CourseSlotId)
                .Index(t => t.ActivityId)
                .Index(t => t.Participant_Id);
            
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        FinishTime = c.DateTime(nullable: false),
                        FacultyMeetingTime = c.DateTime(),
                        DepartmentId = c.Guid(nullable: false),
                        OutreachingDepartmentId = c.Guid(),
                        RoomId = c.Guid(nullable: false),
                        FacultyNoRequired = c.Byte(nullable: false),
                        ParticipantVideoFilename = c.String(maxLength: 256),
                        FeedbackSummaryFilename = c.String(maxLength: 256),
                        CourseFormatId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CourseFormats", t => t.CourseFormatId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .ForeignKey("dbo.Departments", t => t.OutreachingDepartmentId)
                .ForeignKey("dbo.Rooms", t => t.RoomId)
                .Index(t => t.DepartmentId)
                .Index(t => t.OutreachingDepartmentId)
                .Index(t => t.RoomId)
                .Index(t => t.CourseFormatId);
            
            CreateTable(
                "dbo.CourseFormats",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Description = c.String(nullable: false, maxLength: 50),
                        DaysDuration = c.Byte(nullable: false),
                        CourseTypeId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CourseTypes", t => t.CourseTypeId)
                .Index(t => t.CourseTypeId);
            
            CreateTable(
                "dbo.CourseSlots",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MinutesDuration = c.Byte(nullable: false),
                        Order = c.Int(nullable: false),
                        Day = c.Byte(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        SimultaneousStreams = c.Byte(nullable: false),
                        ActivityId = c.Guid(),
                        CourseFormatId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CourseActivities", t => t.ActivityId)
                .ForeignKey("dbo.CourseFormats", t => t.CourseFormatId)
                .Index(t => t.ActivityId)
                .Index(t => t.CourseFormatId);
            
            CreateTable(
                "dbo.CourseActivities",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 128),
                        CourseTypeId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CourseTypes", t => t.CourseTypeId, cascadeDelete: true)
                .Index(t => t.CourseTypeId);
            
            CreateTable(
                "dbo.CourseTypes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Description = c.String(nullable: false, maxLength: 50),
                        Abbreviation = c.String(nullable: false, maxLength: 32),
                        EmersionCategory = c.Int(),
                        InstructorCourseId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CourseTypes", t => t.InstructorCourseId)
                .Index(t => t.InstructorCourseId);
            
            CreateTable(
                "dbo.CourseTypeDepartments",
                c => new
                    {
                        CourseTypeId = c.Guid(nullable: false),
                        DepartmentId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.CourseTypeId, t.DepartmentId })
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .ForeignKey("dbo.CourseTypes", t => t.CourseTypeId, cascadeDelete: true)
                .Index(t => t.CourseTypeId)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 64),
                        Abbreviation = c.String(maxLength: 16),
                        InstitutionId = c.Guid(nullable: false),
                        InvitationLetterFilename = c.String(maxLength: 256),
                        CertificateFilename = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Institutions", t => t.InstitutionId)
                .Index(t => t.InstitutionId);
            
            CreateTable(
                "dbo.CourseParticipants",
                c => new
                    {
                        CourseId = c.Guid(nullable: false),
                        ParticipantId = c.Guid(nullable: false),
                        DepartmentId = c.Guid(nullable: false),
                        ProfessionalRoleId = c.Guid(nullable: false),
                        IsConfirmed = c.Boolean(),
                        IsFaculty = c.Boolean(nullable: false),
                        IsOrganiser = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.CourseId, t.ParticipantId })
                .ForeignKey("dbo.AspNetUsers", t => t.ParticipantId)
                .ForeignKey("dbo.ProfessionalRoles", t => t.ProfessionalRoleId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .ForeignKey("dbo.Courses", t => t.CourseId)
                .Index(t => t.CourseId)
                .Index(t => t.ParticipantId)
                .Index(t => t.DepartmentId)
                .Index(t => t.ProfessionalRoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Email = c.String(nullable: false, maxLength: 256),
                        AlternateEmail = c.String(maxLength: 256),
                        FullName = c.String(nullable: false, maxLength: 256),
                        DefaultDepartmentId = c.Guid(nullable: false),
                        DefaultProfessionalRoleId = c.Guid(nullable: false),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProfessionalRoles", t => t.DefaultProfessionalRoleId)
                .ForeignKey("dbo.Departments", t => t.DefaultDepartmentId, cascadeDelete: true)
                .Index(t => t.DefaultDepartmentId)
                .Index(t => t.DefaultProfessionalRoleId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.CourseScenarioFacultyRoles",
                c => new
                    {
                        CourseId = c.Guid(nullable: false),
                        CourseSlotId = c.Guid(nullable: false),
                        ParticipantId = c.Guid(nullable: false),
                        FacultyScenarioRoleId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.CourseId, t.CourseSlotId, t.ParticipantId, t.FacultyScenarioRoleId })
                .ForeignKey("dbo.FacultyScenarioRoles", t => t.FacultyScenarioRoleId)
                .ForeignKey("dbo.AspNetUsers", t => t.ParticipantId)
                .ForeignKey("dbo.CourseSlots", t => t.CourseSlotId)
                .ForeignKey("dbo.Courses", t => t.CourseId)
                .Index(t => t.CourseId)
                .Index(t => t.CourseSlotId)
                .Index(t => t.ParticipantId)
                .Index(t => t.FacultyScenarioRoleId);
            
            CreateTable(
                "dbo.FacultyScenarioRoles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Description = c.String(nullable: false, maxLength: 50),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CourseTypeScenarioRoles",
                c => new
                    {
                        CourseTypeId = c.Guid(nullable: false),
                        FacultyScenarioRoleId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.CourseTypeId, t.FacultyScenarioRoleId })
                .ForeignKey("dbo.FacultyScenarioRoles", t => t.FacultyScenarioRoleId)
                .ForeignKey("dbo.CourseTypes", t => t.CourseTypeId)
                .Index(t => t.CourseTypeId)
                .Index(t => t.FacultyScenarioRoleId);
            
            CreateTable(
                "dbo.CourseSlotPresenters",
                c => new
                    {
                        CourseId = c.Guid(nullable: false),
                        CourseSlotId = c.Guid(nullable: false),
                        ParticipantId = c.Guid(nullable: false),
                        StreamNumber = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => new { t.CourseId, t.CourseSlotId, t.ParticipantId })
                .ForeignKey("dbo.AspNetUsers", t => t.ParticipantId)
                .ForeignKey("dbo.CourseSlots", t => t.CourseSlotId)
                .ForeignKey("dbo.Courses", t => t.CourseId)
                .Index(t => t.CourseId)
                .Index(t => t.CourseSlotId)
                .Index(t => t.ParticipantId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ProfessionalRoles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Description = c.String(nullable: false, maxLength: 50),
                        Category = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProfessionalRoleInstitutions",
                c => new
                    {
                        ProfessionalRoleId = c.Guid(nullable: false),
                        InstitutionId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProfessionalRoleId, t.InstitutionId })
                .ForeignKey("dbo.Institutions", t => t.InstitutionId, cascadeDelete: true)
                .ForeignKey("dbo.ProfessionalRoles", t => t.ProfessionalRoleId, cascadeDelete: true)
                .Index(t => t.ProfessionalRoleId)
                .Index(t => t.InstitutionId);
            
            CreateTable(
                "dbo.Institutions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        About = c.String(),
                        LocaleCode = c.String(nullable: false, maxLength: 5, fixedLength: true),
                        StandardTimeZone = c.String(maxLength: 40),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cultures", t => t.LocaleCode)
                .Index(t => t.LocaleCode);
            
            CreateTable(
                "dbo.Cultures",
                c => new
                    {
                        LocaleCode = c.String(nullable: false, maxLength: 5, fixedLength: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        CountryCode = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LocaleCode);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        RoleId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Manikins",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Description = c.String(nullable: false, maxLength: 128),
                        DepartmentId = c.Guid(),
                        ModelId = c.Guid(nullable: false),
                        PurchasedNew = c.Boolean(nullable: false),
                        PurchaseDate = c.DateTime(storeType: "date"),
                        LocalCurrencyPurchasePrice = c.Decimal(storeType: "money"),
                        DecommissionDate = c.DateTime(storeType: "date"),
                        DecommissionReason = c.String(maxLength: 512),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .ForeignKey("dbo.ManikinModels", t => t.ModelId)
                .Index(t => t.DepartmentId)
                .Index(t => t.ModelId);
            
            CreateTable(
                "dbo.CourseSlotManikins",
                c => new
                    {
                        CourseId = c.Guid(nullable: false),
                        CourseSlotId = c.Guid(nullable: false),
                        ManikinId = c.Guid(nullable: false),
                        StreamNumber = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => new { t.CourseId, t.CourseSlotId, t.ManikinId })
                .ForeignKey("dbo.Manikins", t => t.ManikinId)
                .ForeignKey("dbo.CourseSlots", t => t.CourseSlotId)
                .ForeignKey("dbo.Courses", t => t.CourseId)
                .Index(t => t.CourseId)
                .Index(t => t.CourseSlotId)
                .Index(t => t.ManikinId);
            
            CreateTable(
                "dbo.ManikinServices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProblemDescription = c.String(maxLength: 1028),
                        ServicedInternally = c.Boolean(nullable: false),
                        Sent = c.DateTime(nullable: false, storeType: "date"),
                        Returned = c.DateTime(storeType: "date"),
                        PriceEstimate = c.Decimal(storeType: "money"),
                        ServiceCost = c.Decimal(nullable: false, storeType: "money"),
                        ManikinId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Manikins", t => t.ManikinId)
                .Index(t => t.ManikinId);
            
            CreateTable(
                "dbo.ManikinModels",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Description = c.String(nullable: false, maxLength: 128),
                        ManufacturerId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ManikinManufacturers", t => t.ManufacturerId)
                .Index(t => t.ManufacturerId);
            
            CreateTable(
                "dbo.ManikinManufacturers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ShortDescription = c.String(maxLength: 32),
                        FullDescription = c.String(maxLength: 64),
                        Directions = c.String(maxLength: 256),
                        DepartmentId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.Scenarios",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        BriefDescription = c.String(nullable: false, maxLength: 64),
                        FullDescription = c.String(maxLength: 256),
                        Complexity = c.Int(),
                        EmersionCategory = c.Int(),
                        TemplateFilename = c.String(maxLength: 256),
                        CourseTypeId = c.Guid(nullable: false),
                        DepartmentId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .ForeignKey("dbo.CourseTypes", t => t.CourseTypeId, cascadeDelete: true)
                .Index(t => t.CourseTypeId)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.CourseSlotScenarios",
                c => new
                    {
                        CourseId = c.Guid(nullable: false),
                        CourseSlotId = c.Guid(nullable: false),
                        ScenarioId = c.Guid(nullable: false),
                        StreamNumber = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => new { t.CourseId, t.CourseSlotId })
                .ForeignKey("dbo.Scenarios", t => t.ScenarioId)
                .ForeignKey("dbo.CourseSlots", t => t.CourseSlotId)
                .ForeignKey("dbo.Courses", t => t.CourseId)
                .Index(t => t.CourseId)
                .Index(t => t.CourseSlotId)
                .Index(t => t.ScenarioId);
            
            CreateTable(
                "dbo.ScenarioResources",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ScenarioId = c.Guid(nullable: false),
                        Description = c.String(),
                        ResourceFilename = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Scenarios", t => t.ScenarioId, cascadeDelete: true)
                .Index(t => t.ScenarioId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ChosenTeachingResources", "ActivityId", "dbo.Activities");
            DropForeignKey("dbo.CourseSlotScenarios", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.CourseSlotPresenters", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.CourseSlotManikins", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.CourseScenarioFacultyRoles", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.CourseParticipants", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.CourseSlots", "CourseFormatId", "dbo.CourseFormats");
            DropForeignKey("dbo.CourseSlotScenarios", "CourseSlotId", "dbo.CourseSlots");
            DropForeignKey("dbo.CourseSlotPresenters", "CourseSlotId", "dbo.CourseSlots");
            DropForeignKey("dbo.CourseSlotManikins", "CourseSlotId", "dbo.CourseSlots");
            DropForeignKey("dbo.CourseScenarioFacultyRoles", "CourseSlotId", "dbo.CourseSlots");
            DropForeignKey("dbo.ChosenTeachingResources", "CourseSlotId", "dbo.CourseSlots");
            DropForeignKey("dbo.Scenarios", "CourseTypeId", "dbo.CourseTypes");
            DropForeignKey("dbo.CourseTypes", "InstructorCourseId", "dbo.CourseTypes");
            DropForeignKey("dbo.CourseTypeScenarioRoles", "CourseTypeId", "dbo.CourseTypes");
            DropForeignKey("dbo.CourseTypeDepartments", "CourseTypeId", "dbo.CourseTypes");
            DropForeignKey("dbo.Scenarios", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.ScenarioResources", "ScenarioId", "dbo.Scenarios");
            DropForeignKey("dbo.CourseSlotScenarios", "ScenarioId", "dbo.Scenarios");
            DropForeignKey("dbo.Rooms", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Courses", "RoomId", "dbo.Rooms");
            DropForeignKey("dbo.AspNetUsers", "DefaultDepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Courses", "OutreachingDepartmentId", "dbo.Departments");
            DropForeignKey("dbo.ManikinModels", "ManufacturerId", "dbo.ManikinManufacturers");
            DropForeignKey("dbo.Manikins", "ModelId", "dbo.ManikinModels");
            DropForeignKey("dbo.ManikinServices", "ManikinId", "dbo.Manikins");
            DropForeignKey("dbo.Manikins", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.CourseSlotManikins", "ManikinId", "dbo.Manikins");
            DropForeignKey("dbo.CourseTypeDepartments", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Courses", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.CourseParticipants", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProfessionalRoleInstitutions", "ProfessionalRoleId", "dbo.ProfessionalRoles");
            DropForeignKey("dbo.ProfessionalRoleInstitutions", "InstitutionId", "dbo.Institutions");
            DropForeignKey("dbo.Departments", "InstitutionId", "dbo.Institutions");
            DropForeignKey("dbo.Institutions", "LocaleCode", "dbo.Cultures");
            DropForeignKey("dbo.AspNetUsers", "DefaultProfessionalRoleId", "dbo.ProfessionalRoles");
            DropForeignKey("dbo.CourseParticipants", "ProfessionalRoleId", "dbo.ProfessionalRoles");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CourseSlotPresenters", "ParticipantId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CourseScenarioFacultyRoles", "ParticipantId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CourseTypeScenarioRoles", "FacultyScenarioRoleId", "dbo.FacultyScenarioRoles");
            DropForeignKey("dbo.CourseScenarioFacultyRoles", "FacultyScenarioRoleId", "dbo.FacultyScenarioRoles");
            DropForeignKey("dbo.CourseParticipants", "ParticipantId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ChosenTeachingResources", "Participant_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.CourseFormats", "CourseTypeId", "dbo.CourseTypes");
            DropForeignKey("dbo.CourseActivities", "CourseTypeId", "dbo.CourseTypes");
            DropForeignKey("dbo.CourseSlots", "ActivityId", "dbo.CourseActivities");
            DropForeignKey("dbo.Activities", "CourseActivityId", "dbo.CourseActivities");
            DropForeignKey("dbo.Courses", "CourseFormatId", "dbo.CourseFormats");
            DropForeignKey("dbo.ChosenTeachingResources", "CourseId", "dbo.Courses");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.ScenarioResources", new[] { "ScenarioId" });
            DropIndex("dbo.CourseSlotScenarios", new[] { "ScenarioId" });
            DropIndex("dbo.CourseSlotScenarios", new[] { "CourseSlotId" });
            DropIndex("dbo.CourseSlotScenarios", new[] { "CourseId" });
            DropIndex("dbo.Scenarios", new[] { "DepartmentId" });
            DropIndex("dbo.Scenarios", new[] { "CourseTypeId" });
            DropIndex("dbo.Rooms", new[] { "DepartmentId" });
            DropIndex("dbo.ManikinModels", new[] { "ManufacturerId" });
            DropIndex("dbo.ManikinServices", new[] { "ManikinId" });
            DropIndex("dbo.CourseSlotManikins", new[] { "ManikinId" });
            DropIndex("dbo.CourseSlotManikins", new[] { "CourseSlotId" });
            DropIndex("dbo.CourseSlotManikins", new[] { "CourseId" });
            DropIndex("dbo.Manikins", new[] { "ModelId" });
            DropIndex("dbo.Manikins", new[] { "DepartmentId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.Institutions", new[] { "LocaleCode" });
            DropIndex("dbo.ProfessionalRoleInstitutions", new[] { "InstitutionId" });
            DropIndex("dbo.ProfessionalRoleInstitutions", new[] { "ProfessionalRoleId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.CourseSlotPresenters", new[] { "ParticipantId" });
            DropIndex("dbo.CourseSlotPresenters", new[] { "CourseSlotId" });
            DropIndex("dbo.CourseSlotPresenters", new[] { "CourseId" });
            DropIndex("dbo.CourseTypeScenarioRoles", new[] { "FacultyScenarioRoleId" });
            DropIndex("dbo.CourseTypeScenarioRoles", new[] { "CourseTypeId" });
            DropIndex("dbo.CourseScenarioFacultyRoles", new[] { "FacultyScenarioRoleId" });
            DropIndex("dbo.CourseScenarioFacultyRoles", new[] { "ParticipantId" });
            DropIndex("dbo.CourseScenarioFacultyRoles", new[] { "CourseSlotId" });
            DropIndex("dbo.CourseScenarioFacultyRoles", new[] { "CourseId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "DefaultProfessionalRoleId" });
            DropIndex("dbo.AspNetUsers", new[] { "DefaultDepartmentId" });
            DropIndex("dbo.CourseParticipants", new[] { "ProfessionalRoleId" });
            DropIndex("dbo.CourseParticipants", new[] { "DepartmentId" });
            DropIndex("dbo.CourseParticipants", new[] { "ParticipantId" });
            DropIndex("dbo.CourseParticipants", new[] { "CourseId" });
            DropIndex("dbo.Departments", new[] { "InstitutionId" });
            DropIndex("dbo.CourseTypeDepartments", new[] { "DepartmentId" });
            DropIndex("dbo.CourseTypeDepartments", new[] { "CourseTypeId" });
            DropIndex("dbo.CourseTypes", new[] { "InstructorCourseId" });
            DropIndex("dbo.CourseActivities", new[] { "CourseTypeId" });
            DropIndex("dbo.CourseSlots", new[] { "CourseFormatId" });
            DropIndex("dbo.CourseSlots", new[] { "ActivityId" });
            DropIndex("dbo.CourseFormats", new[] { "CourseTypeId" });
            DropIndex("dbo.Courses", new[] { "CourseFormatId" });
            DropIndex("dbo.Courses", new[] { "RoomId" });
            DropIndex("dbo.Courses", new[] { "OutreachingDepartmentId" });
            DropIndex("dbo.Courses", new[] { "DepartmentId" });
            DropIndex("dbo.ChosenTeachingResources", new[] { "Participant_Id" });
            DropIndex("dbo.ChosenTeachingResources", new[] { "ActivityId" });
            DropIndex("dbo.ChosenTeachingResources", new[] { "CourseSlotId" });
            DropIndex("dbo.ChosenTeachingResources", new[] { "CourseId" });
            DropIndex("dbo.Activities", new[] { "CourseActivityId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ScenarioResources");
            DropTable("dbo.CourseSlotScenarios");
            DropTable("dbo.Scenarios");
            DropTable("dbo.Rooms");
            DropTable("dbo.ManikinManufacturers");
            DropTable("dbo.ManikinModels");
            DropTable("dbo.ManikinServices");
            DropTable("dbo.CourseSlotManikins");
            DropTable("dbo.Manikins");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.Cultures");
            DropTable("dbo.Institutions");
            DropTable("dbo.ProfessionalRoleInstitutions");
            DropTable("dbo.ProfessionalRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.CourseSlotPresenters");
            DropTable("dbo.CourseTypeScenarioRoles");
            DropTable("dbo.FacultyScenarioRoles");
            DropTable("dbo.CourseScenarioFacultyRoles");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.CourseParticipants");
            DropTable("dbo.Departments");
            DropTable("dbo.CourseTypeDepartments");
            DropTable("dbo.CourseTypes");
            DropTable("dbo.CourseActivities");
            DropTable("dbo.CourseSlots");
            DropTable("dbo.CourseFormats");
            DropTable("dbo.Courses");
            DropTable("dbo.ChosenTeachingResources");
            DropTable("dbo.Activities");
        }
    }
}
