using SM.Metadata;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SM.Dto
{

    internal class MedSimDtoContextPretender : DbContext
    {
        static MedSimDtoContextPretender()
        {
            // Prevent attempt to initialize a database for this context
            Database.SetInitializer<MedSimDtoContextPretender>(null);
        }

        //  Dto versions of these Northwind Model classes
        public virtual DbSet<CountryDto> Countries { get; set; }
        public virtual DbSet<CountryLocaleCodeDto> CountryLocales { get; set; }
        public virtual DbSet<DepartmentDto> Departments { get; set; }
        public virtual DbSet<ScenarioRoleDescriptionDto> SenarioRoles { get; set; }
        public virtual DbSet<InstitutionDto> Institutions { get; set; }
        public virtual DbSet<ManequinDto> Manequins { get; set; }
        public virtual DbSet<ManequinManufacturerDto> ManequinManufacturers { get; set; }
        public virtual DbSet<ParticipantDto> Participants { get; set; }
        public virtual DbSet<ProfessionalRoleDto> ProfessionalRoles { get; set; }
        public virtual DbSet<ScenarioDto> Scenarios { get; set; }
        public virtual DbSet<ScenarioResourceDto> ScenarioResources { get; set; }
        public virtual DbSet<CourseDto> Courses { get; set; }
        public virtual DbSet<CourseParticipantDto> CourseParticipants { get; set; }
        public virtual DbSet<CourseTypeDto> CourseTypes { get; set; }
        public virtual DbSet<CourseSlotDto> CourseEvents { get; set; }
        public virtual DbSet<ScenarioSlotDto> CourseScenarios { get; set; }
        public virtual DbSet<CourseTeachingResourceDto> CourseTeachingResources { get; set; }
        public virtual DbSet<CourseSlotPresenterDto> CourseSlotPresenters { get; set; }
        public virtual DbSet<ScenarioFacultyRoleDto> ScenarioFacultyRoles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Add(new FixedLengthAttributeConvention());

            modelBuilder.Entity<CountryDto>()
                .Property(e => e.Code)
                .IsFixedLength();

            modelBuilder.Entity<CountryDto>()
                .HasMany(e => e.ProfessionalRoles)
                .WithMany(e => e.Countries)
                .Map(m => m.ToTable("CountryProfessionalRole").MapLeftKey("CountryCode").MapRightKey("ProfessionalRoleId"));

            modelBuilder.Entity<CountryDto>()
                .HasMany(e => e.CountryLocales)
                .WithRequired(e => e.Country)
                .HasForeignKey(e => e.CountryCode);

            modelBuilder.Entity<CourseDto>()
                .HasMany(e => e.CourseParticipants)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseDto>()
                .HasMany(e => e.ScenarioFacultyRoles)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseSlotDto>()
                .HasMany(e => e.CourseTypes)
                .WithMany(e => e.CourseEvents);

            modelBuilder.Entity<CourseSlotDto>()
                .HasMany(e => e.DefaultResources)
                .WithRequired(e => e.CourseSlot)
                .HasForeignKey(e => e.CourseSlotId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseTypeDto>()
                .HasMany(e => e.Scenarios)
                .WithRequired(e => e.CourseType)
                .HasForeignKey(e => e.CourseTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseTypeDto>()
                .HasMany(e => e.CourseEvents)
                .WithMany(e => e.CourseTypes);

            modelBuilder.Entity<CourseTypeDto>()
                .HasMany(e => e.Courses)
                .WithRequired(e => e.CourseType)
                .HasForeignKey(e => e.CourseTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DepartmentDto>()
                .HasMany(e => e.CourseTypes)
                .WithMany(e => e.Departments);

            modelBuilder.Entity<DepartmentDto>()
                .HasMany(e => e.Participants)
                .WithRequired(e => e.Department)
                .HasForeignKey(e => e.DefaultDepartmentId);

            modelBuilder.Entity<DepartmentDto>()
                .HasMany(e => e.Courses)
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

            modelBuilder.Entity<InstitutionDto>()
                .Property(e => e.CountryCode)
                .IsFixedLength();

            modelBuilder.Entity<InstitutionDto>()
                .HasMany(e => e.Departments)
                .WithRequired(e => e.Institution)
                .HasForeignKey(e => e.InstitutionId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ParticipantDto>()
                .HasMany(e => e.CourseParticipants)
                .WithRequired(e => e.Participant)
                .HasForeignKey(e => e.ParticipantId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ParticipantDto>()
                .HasMany(e => e.ScenarioFacultyRoles)
                .WithRequired(e => e.FacultyMember)
                .HasForeignKey(e => e.FacultyMemberId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ParticipantDto>()
                .HasMany(e => e.CourseSlotPresentations)
                .WithRequired(e => e.Presenter)
                .HasForeignKey(e => e.PresenterId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProfessionalRoleDto>()
                .HasMany(e => e.Participants)
                .WithRequired(e => e.ProfessionalRole)
                .HasForeignKey(e => e.DefaultProfessionalRoleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RoomDto>()
                .HasMany(e => e.Courses)
                .WithRequired(e => e.Room)
                .HasForeignKey(e => e.RoomId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ScenarioDto>()
                .HasMany(e => e.ScenarioFacultyRoles)
                .WithRequired(e => e.Scenario)
                .HasForeignKey(e => e.ScenarioId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ScenarioRoleDescriptionDto>()
                .HasMany(e => e.ScenarioFacultyRoles)
                .WithRequired(e => e.Role)
                .HasForeignKey(e => e.RoleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ScenarioRoleDescriptionDto>()
                .HasMany(e => e.CourseTypes)
                .WithMany(e => e.ScenarioRoles);

            modelBuilder.Entity<ScenarioSlotDto>()
                .HasMany(e => e.CourseTypes)
                .WithMany(e => e.ScenarioEvents);

            base.OnModelCreating(modelBuilder);

        }
    }
}
