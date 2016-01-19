namespace SM.DataAccess
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MedSimDbContext : DbContext
    {
        public MedSimDbContext()
            : base("name=MedSimData")
        {
            Database.SetInitializer<MedSimDbContext>(new InitialiseMedSim());
        }

        public virtual DbSet<Participant> Participants { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<SessionRoleType> SessionRoles { get; set; }
        public virtual DbSet<ScenarioRoleType> SenarioRoles { get; set; }
        public virtual DbSet<Hospital> Hospitals { get; set; }
        public virtual DbSet<InstructorCourse> InstructorCourses { get; set; }
        public virtual DbSet<InstructorCourseParticipant> InstructorCourseParticipants { get; set; }
        public virtual DbSet<Manequin> Manequins { get; set; }
        public virtual DbSet<ProfessionalRole> ProfessionalRoles { get; set; }
        public virtual DbSet<Scenario> Scenarios { get; set; }
        public virtual DbSet<ScenarioResource> ScenarioResources { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<SessionParticipant> SessionParticipants { get; set; }
        public virtual DbSet<SessionResource> SessionResourses { get; set; }
        public virtual DbSet<SessionType> SessionTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Participant>()
                .HasMany(e => e.InstructorCourses)
                .WithRequired(e => e.Participant)
                .HasForeignKey(e => e.ParticipantId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Participant>()
                .HasMany(e => e.SessionParticipants)
                .WithRequired(e => e.Participant)
                .HasForeignKey(e => e.ParticipantId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Country>()
                .Property(e => e.Code)
                .IsFixedLength();

            modelBuilder.Entity<Country>()
                .HasMany(e => e.ProfessionalRoles)
                .WithMany(e => e.Countries)
                .Map(m => m.ToTable("CountryProfessionalRole").MapLeftKey("CountryCode").MapRightKey("ProfessionalRoleId"));

            modelBuilder.Entity<Department>()
                .HasMany(e => e.Participants)
                .WithRequired(e => e.Department)
                .HasForeignKey(e => e.DefaultDepartmentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.Sessions)
                .WithOptional(e => e.OutreachingDepartment)
                .HasForeignKey(e => e.OutreachingDepartmentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Hospital>()
                .Property(e => e.CountryCode)
                .IsFixedLength();

            modelBuilder.Entity<Hospital>()
                .HasMany(e => e.Departments)
                .WithRequired(e => e.Hospital)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<InstructorCourse>()
                .HasMany(e => e.InstructorCourseParticipants)
                .WithRequired(e => e.InstructorCourse)
                .HasForeignKey(e => e.CourseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProfessionalRole>()
                .HasMany(e => e.Participants)
                .WithRequired(e => e.ProfessionalRole)
                .HasForeignKey(e => e.DefaultProfessionalRoleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Session>()
                .HasMany(e => e.SessionParticipants)
                .WithRequired(e => e.Session)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SessionType>()
                .HasMany(e => e.Scenarios)
                .WithRequired(e => e.SessionType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SessionType>()
                .HasMany(e => e.Sessions)
                .WithRequired(e => e.SessionType)
                .WillCascadeOnDelete(false);
        }
    }
}
