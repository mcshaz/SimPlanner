using SP.Metadata;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SP.Dto
{

    internal class MedSimDtoContextPretender : DbContext
    {
        static MedSimDtoContextPretender()
        {
            // Prevent attempt to initialize a database for this context
            Database.SetInitializer<MedSimDtoContextPretender>(null);
        }
        public virtual DbSet<ActivityDto> Activities { get; set; }
        public virtual DbSet<CandidatePrereadingDto> CandidatePrereading { get; set; }
        public virtual DbSet<CultureDto> cultures { get; set; }
        public virtual DbSet<CourseDto> Courses { get; set; }
        public virtual DbSet<CourseActivityDto> CourseActivities { get; set; }
        public virtual DbSet<CourseFormatDto> CourseFormats { get; set; }
        public virtual DbSet<CourseParticipantDto> CourseParticipants { get; set; }
        public virtual DbSet<CourseScenarioFacultyRoleDto> CourseScenarioFacultyRoles { get; set; }
        public virtual DbSet<CourseSlotDto> CourseSlots { get; set; }
        public virtual DbSet<CourseSlotActivityDto> CourseSlotActivities { get; set; }
        public virtual DbSet<CourseSlotManikinDto> CourseSlotManikins { get; set; }
        public virtual DbSet<CourseSlotPresenterDto> CourseSlotPresenters { get; set; }
        public virtual DbSet<CourseTypeDto> CourseTypes { get; set; }
        public virtual DbSet<CourseTypeScenarioRoleDto> CourseTypeScenarioRoles { get; set; }
        public virtual DbSet<DepartmentDto> Departments { get; set; }
        public virtual DbSet<FacultyScenarioRoleDto> FacultyScenarioRoles { get; set; }
        public virtual DbSet<HotDrinkDto> HotDrinks { get; set; }
        public virtual DbSet<InstitutionDto> Institutions { get; set; }
        public virtual DbSet<ManikinDto> Manikins { get; set; }
        public virtual DbSet<ManikinManufacturerDto> ManikinManufacturers { get; set; }
        public virtual DbSet<ProfessionalRoleDto> ProfessionalRoles { get; set; }
        public virtual DbSet<ProfessionalRoleInstitutionDto> ProfessionalRoleInstitutions { get; set; }
        //public virtual DbSet<RoleDto> Roles { get; set; }
        public virtual DbSet<RoomDto> Rooms { get; set; }
        public virtual DbSet<ScenarioDto> Scenarios { get; set; }
        public virtual DbSet<ScenarioResourceDto> ScenarioResources { get; set; }
        public virtual DbSet<UserRoleDto> UserRoles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Add(new FixedLengthAttributeConvention());

            modelBuilder.Entity<ActivityDto>()
                .HasMany(e => e.CourseSlotActivities)
                .WithOptional(e => e.Activity)
                .HasForeignKey(e => e.ActivityId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CultureDto>()
                .HasMany(e => e.Institutions)
                .WithOptional(e => e.Culture) //NOTE this is different from the DTO on purpose - if the entity doesn't exist, we have the tools to create it first
                .HasForeignKey(e => e.LocaleCode)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseDto>()
                .HasMany(e => e.CourseParticipants)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseDto>()
                .HasMany(e => e.CourseScenarioFacultyRoles)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseDto>()
                .HasMany(e => e.CourseSlotActivities)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseDto>()
                .HasMany(e => e.CourseSlotManikins)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseDto>()
                .HasMany(e => e.CourseSlotPresenters)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseDto>()
                .HasMany(e => e.CourseDays)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseActivityDto>()
                    .HasMany(e => e.CourseSlots)
                    .WithOptional(e => e.Activity)
                    .HasForeignKey(e => e.ActivityId)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseActivityDto>()
                .HasMany(e => e.ActivityChoices)
                .WithRequired(e => e.CourseActivity)
                .HasForeignKey(e => e.CourseActivityId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseFormatDto>()
                .HasMany(e => e.CourseSlots)
                .WithRequired(e => e.CourseFormat)
                .HasForeignKey(e => e.CourseFormatId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseFormatDto>()
                .HasMany(e => e.Courses)
                .WithRequired(e => e.CourseFormat)
                .HasForeignKey(e => e.CourseFormatId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseParticipantDto>()
                .Ignore(e => e.IsEmailed);

            modelBuilder.Entity<CourseSlotDto>()
                .HasMany(e => e.CourseSlotPresenters)
                .WithRequired(e => e.CourseSlot)
                .HasForeignKey(e => e.CourseSlotId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseSlotDto>()
                .HasMany(e => e.CourseScenarioFacultyRoles)
                .WithRequired(e => e.CourseSlot)
                .HasForeignKey(e => e.CourseSlotId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseSlotDto>()
                .HasMany(e => e.CourseSlotActivities)
                .WithRequired(e => e.CourseSlot)
                .HasForeignKey(e => e.CourseSlotId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseSlotDto>()
                .HasMany(e => e.CourseSlotManikins)
                .WithRequired(e => e.CourseSlot)
                .HasForeignKey(e => e.CourseSlotId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseTypeDto>()
                .HasMany(e => e.CandidatePrereadings)
                .WithRequired(e => e.CourseType)
                .HasForeignKey(e => e.CourseTypeId);

            modelBuilder.Entity<CourseTypeDto>()
                .HasMany(e => e.CourseTypeScenarioRoles)
                .WithRequired(e => e.CourseType)
                .HasForeignKey(e => e.CourseTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseTypeDto>()
                .HasMany(e => e.CourseFormats)
                .WithRequired(e => e.CourseType)
                .HasForeignKey(e => e.CourseTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseTypeDto>()
                .HasOptional(e => e.InstructorCourse)
                .WithMany()
                .HasForeignKey(e => e.InstructorCourseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseTypeDto>()
                .HasMany(e => e.Scenarios)
                .WithRequired(e => e.CourseType)
                .HasForeignKey(e => e.CourseTypeId);

            modelBuilder.Entity<CourseTypeDto>()
                .HasMany(e => e.CourseActivities)
                .WithRequired(e => e.CourseType)
                .HasForeignKey(e => e.CourseTypeId);

            modelBuilder.Entity<CourseTypeDto>()
                .HasMany(e => e.CourseTypeDepartments)
                .WithRequired(e => e.CourseType)
                .HasForeignKey(e => e.CourseTypeId);

            modelBuilder.Entity<DepartmentDto>()
                .HasMany(e => e.CourseTypeDepartments)
                .WithRequired(e => e.Department)
                .HasForeignKey(e => e.DepartmentId);

            modelBuilder.Entity<DepartmentDto>()
                .HasMany(e => e.Participants)
                .WithRequired(e => e.Department)
                .HasForeignKey(e => e.DefaultDepartmentId);

            modelBuilder.Entity<DepartmentDto>()
                .HasMany(e => e.Courses)
                .WithRequired(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DepartmentDto>()
                .HasMany(e => e.OutreachCourses)
                .WithOptional(e => e.OutreachingDepartment)
                .HasForeignKey(e => e.OutreachingDepartmentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DepartmentDto>()
                .HasMany(e => e.Scenarios)
                .WithRequired(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DepartmentDto>()
                .HasMany(e => e.Rooms)
                .WithRequired(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DepartmentDto>()
                .HasMany(e => e.CourseParticipants)
                .WithRequired(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DepartmentDto>()
                .HasMany(e => e.Manikins)
                .WithRequired(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FacultyScenarioRoleDto>()
                .HasMany(e => e.CourseScenarioFacultyRoles)
                .WithRequired(e => e.FacultyScenarioRole)
                .HasForeignKey(e => e.FacultyScenarioRoleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FacultyScenarioRoleDto>()
                .HasMany(e => e.CourseTypeScenarioRoles)
                .WithRequired(e => e.FacultyScenarioRole)
                .HasForeignKey(e => e.FacultyScenarioRoleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HotDrinkDto>()
                .HasMany(e => e.Participants)
                .WithOptional(e => e.DrinkPreference)
                .HasForeignKey(e => e.DrinkPreferenceId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<InstitutionDto>()
                .Property(e => e.LocaleCode)
                .IsFixedLength();

            modelBuilder.Entity<InstitutionDto>()
                .HasMany(e => e.Departments)
                .WithRequired(e => e.Institution)
                .HasForeignKey(e => e.InstitutionId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<InstitutionDto>()
                .HasMany(e => e.ProfessionalRoleInstitutions)
                .WithRequired(e => e.Institution)
                .HasForeignKey(e => e.InstitutionId);

            modelBuilder.Entity<ManikinDto>()
                .HasMany(e => e.ManikinServices)
                .WithRequired(e => e.Manikin)
                .HasForeignKey(e => e.ManikinId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ManikinDto>()
                .HasMany(e => e.CourseSlotManikins)
                .WithRequired(e => e.Manikin)
                .HasForeignKey(e => e.ManikinId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ManikinManufacturerDto>()
                .HasMany(e => e.ManikinModels)
                .WithRequired(e => e.Manufacturer)
                .HasForeignKey(e => e.ManufacturerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ManikinModelDto>()
                .HasMany(e => e.Manikins)
                .WithRequired(e => e.Model)
                .HasForeignKey(e => e.ModelId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ParticipantDto>()
                .HasMany(e => e.CourseParticipants)
                .WithRequired(e => e.Participant)
                .HasForeignKey(e => e.ParticipantId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ParticipantDto>()
                .HasMany(e => e.CourseScenarioFacultyRoles)
                .WithRequired(e => e.Participant)
                .HasForeignKey(e => e.ParticipantId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ParticipantDto>()
                .HasMany(e => e.CourseSlotPresentations)
                .WithRequired(e => e.Participant)
                .HasForeignKey(e => e.ParticipantId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ParticipantDto>()
                .HasMany(e => e.Roles)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProfessionalRoleDto>()
                .HasMany(e => e.Participants)
                .WithRequired(e => e.ProfessionalRole)
                .HasForeignKey(e => e.DefaultProfessionalRoleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProfessionalRoleDto>()
                .HasMany(e => e.CourseParticipants)
                .WithRequired(e => e.ProfessionalRole)
                .HasForeignKey(e => e.ProfessionalRoleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProfessionalRoleDto>()
                .HasMany(e => e.ProfessionalRoleInstitutions)
                .WithRequired(e => e.ProfessionalRole)
                .HasForeignKey(e => e.ProfessionalRoleId);
            /*
            modelBuilder.Entity<RoleDto>()
                .HasMany(e => e.UserRoles)
                .WithRequired(e => e.Role)
                .HasForeignKey(e => e.RoleId)
                .WillCascadeOnDelete(false);
            */
            modelBuilder.Entity<RoomDto>()
                .HasMany(e => e.Courses)
                .WithRequired(e => e.Room)
                .HasForeignKey(e => e.RoomId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RoomDto>()
                .HasMany(e => e.FacultyMeetings)
                .WithOptional(e => e.FacultyMeetingRoom)
                .HasForeignKey(e => e.FacultyMeetingRoomId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ScenarioDto>()
                .HasMany(e => e.CourseSlotScenarios)
                .WithOptional(e => e.Scenario)
                .HasForeignKey(e => e.ScenarioId)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<ManikinModelDto> ManikinModelDtoes { get; set; }
    }
}
