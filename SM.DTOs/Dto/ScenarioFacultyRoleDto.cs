using SM.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
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

    }
}
