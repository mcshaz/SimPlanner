namespace SM.DataAccess
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using SM.Metadata;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    public partial class MedSimDbContext : IdentityDbContext<Participant, AspNetRole,
        Guid, AspNetUserLogin,AspNetUserRole, AspNetUserClaim>
    {
        public MedSimDbContext()
            : base("DefaultConnection")
        {
        }

        static MedSimDbContext()
        {
            Database.SetInitializer(new InitialiseMedSim());
        }

        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<CountryLocaleCode> CountryLocaleCodes { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<ScenarioRoleDescription> SenarioRoles { get; set; }
        public virtual DbSet<Institution> Institutions { get; set; }
        public virtual DbSet<Manequin> Manequins { get; set; }
        public virtual DbSet<ManequinManufacturer> ManequinManufacturers { get; set; }
        public virtual DbSet<ProfessionalRole> ProfessionalRoles { get; set; }
        public virtual DbSet<Scenario> Scenarios { get; set; }
        public virtual DbSet<ScenarioResource> ScenarioResources { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<CourseParticipant> CourseParticipants { get; set; }
        public virtual DbSet<CourseType> CourseTypes { get; set; }
        public virtual DbSet<CourseSlot> CourseEvents { get; set; }
        public virtual DbSet<ScenarioSlot> CourseScenarios { get; set; }
        public virtual DbSet<CourseTeachingResource> CourseTeachingResources { get; set; }
        public virtual DbSet<CourseSlotPresenter> CourseSlotPresenters { get; set; }
        public virtual DbSet<ScenarioFacultyRole> ScenarioFacultyRoles { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }

        #region overrides //overriding to allow access without referencing aspnet.identity.entityframework assembly
        public override IDbSet<Participant> Users
        {
            get
            {
                return base.Users;
            }

            set
            {
                base.Users = value;
            }
        }
        #endregion //ovrerides


        public static MedSimDbContext Create()
        {
            return new MedSimDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Configuration.LazyLoadingEnabled = false;
            //Configuration.ProxyCreationEnabled = false;

            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Add(new FixedLengthAttributeConvention());

            modelBuilder.Entity<Country>()
                .HasMany(e => e.ProfessionalRoles)
                .WithMany(e => e.Countries)
                .Map(m => m.ToTable("CountryProfessionalRole").MapLeftKey("CountryCode").MapRightKey("ProfessionalRoleId"));

            modelBuilder.Entity<Country>()
                .HasMany(e => e.CountryLocales)
                .WithRequired(e => e.Country)
                .HasForeignKey(e=>e.CountryCode);

            modelBuilder.Entity<Course>()
                .HasMany(e => e.CourseParticipants)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Course>()
                .HasMany(e => e.ScenarioFacultyRoles)
                .WithRequired(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseSlot>()
                .HasMany(e => e.CourseTypes)
                .WithMany(e => e.CourseEvents);

            modelBuilder.Entity<CourseSlot>()
                .HasMany(e => e.DefaultResources)
                .WithRequired(e => e.CourseSlot)
                .HasForeignKey(e => e.CourseSlotId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseType>()
                .HasMany(e => e.Scenarios)
                .WithRequired(e => e.CourseType)
                .HasForeignKey(e=>e.CourseTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CourseType>()
                .HasMany(e => e.CourseEvents)
                .WithMany(e => e.CourseTypes);

            modelBuilder.Entity<CourseType>()
                .HasMany(e => e.Courses)
                .WithRequired(e => e.CourseType)
                .HasForeignKey(e=>e.CourseTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.CourseTypes)
                .WithMany(e=>e.Departments);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.Participants)
                .WithRequired(e => e.Department)
                .HasForeignKey(e=>e.DefaultDepartmentId);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.Courses)
                .WithOptional(e => e.OutreachingDepartment)
                .HasForeignKey(e => e.OutreachingDepartmentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.Scenarios)
                .WithRequired(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.Rooms)
                .WithRequired(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Institution>()
                .Property(e => e.CountryCode)
                .IsFixedLength();

            modelBuilder.Entity<Institution>()
                .HasMany(e => e.Departments)
                .WithRequired(e => e.Institution)
                .HasForeignKey(e=>e.InstitutionId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Participant>()
                .HasMany(e => e.CourseParticipants)
                .WithRequired(e => e.Participant)
                .HasForeignKey(e => e.ParticipantId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Participant>()
                .HasMany(e => e.ScenarioFacultyRoles)
                .WithRequired(e => e.FacultyMember)
                .HasForeignKey(e => e.FacultyMemberId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Participant>()
                .HasMany(e => e.CourseSlotPresentations)
                .WithRequired(e => e.Presenter)
                .HasForeignKey(e => e.PresenterId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProfessionalRole>()
                .HasMany(e => e.Participants)
                .WithRequired(e => e.ProfessionalRole)
                .HasForeignKey(e => e.DefaultProfessionalRoleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Room>()
                .HasMany(e => e.Courses)
                .WithRequired(e => e.Room)
                .HasForeignKey(e => e.RoomId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Scenario>()
                .HasMany(e => e.ScenarioFacultyRoles)
                .WithRequired(e => e.Scenario)
                .HasForeignKey(e=>e.ScenarioId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ScenarioRoleDescription>()
                .HasMany(e => e.ScenarioFacultyRoles)
                .WithRequired(e => e.Role)
                .HasForeignKey(e => e.RoleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ScenarioRoleDescription>()
                .HasMany(e => e.CourseTypes)
                .WithMany(e => e.ScenarioRoles);

            modelBuilder.Entity<ScenarioSlot>()
                .HasMany(e => e.CourseTypes)
                .WithMany(e => e.ScenarioEvents);

            base.OnModelCreating(modelBuilder);

        }

        /*
        private static void buildAuth(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.Roles)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.Claims)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.Logins)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetRole>()
                .HasKey(e=>e.Id)
                .HasMany(e => e.Users)
                .WithRequired(e => e.Role)
                .HasForeignKey(e => e.RoleId);

            modelBuilder.Entity<AspNetUserClaim>()
                .HasKey(e => e.Id);

            //suspect chances of the oauth provider keys overlapping is pretty remote
            modelBuilder.Entity<AspNetUserLogin>()
                .HasKey(e => new { e.LoginProvider, e.ProviderKey, e.UserId});

            modelBuilder.Entity<AspNetUserRole>()
                .HasKey(e => new { e.RoleId, e.UserId });

        }
        */

        private SanitizeStringProperties _sanitizeHtml { get; set; }
        private SanitizeStringProperties SanitizeHtml
        {
            get { return _sanitizeHtml ?? (_sanitizeHtml = new SanitizeStringProperties()); }
        }

        public override int SaveChanges()
        {
            SanitizeHtml.ForEntities(ChangeTracker);
            return base.SaveChanges();
        }


    }
}
