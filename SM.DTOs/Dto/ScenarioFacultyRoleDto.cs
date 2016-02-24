using SM.DataAccess;
using SM.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
namespace SM.Dto
{
    [MetadataType(typeof(ScenarioFacultyRoleMetadata))]
    public class ScenarioFacultyRoleDto
    {
        public Guid CourseId { get; set; }
        public Guid ScenarioId { get; set; }
        public Guid FacultyMemberId { get; set; }
        public Guid RoleId { get; set; }
        public CourseDto Course { get; set; }
        public ScenarioDto Scenario { get; set; }
        public ScenarioRoleDescriptionDto Role { get; set; }
        public ParticipantDto FacultyMember { get; set; }

        internal static Func<ScenarioFacultyRoleDto, ScenarioFacultyRole>  mapToRepo = m => new ScenarioFacultyRole {
            CourseId = m.CourseId,
            ScenarioId = m.ScenarioId,
            FacultyMemberId = m.FacultyMemberId,
            RoleId = m.RoleId,
            //Course = m.Course,
            //Scenario = m.Scenario,
            //Role = m.Role,
            //FacultyMember = m.FacultyMember
        };

        internal static Expression<Func<ScenarioFacultyRole, ScenarioFacultyRoleDto>> mapFromRepo= m => new ScenarioFacultyRoleDto
        {
            CourseId = m.CourseId,
            ScenarioId = m.ScenarioId,
            FacultyMemberId = m.FacultyMemberId,
            RoleId = m.RoleId,
            //Course = m.Course,
            //Scenario = m.Scenario,
            //Role = m.Role,
            //FacultyMember = m.FacultyMember
        };
    }
}
