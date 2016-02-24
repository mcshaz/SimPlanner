using System.Data.Entity;

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
    }
}
