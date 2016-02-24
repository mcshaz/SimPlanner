using SM.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SM.DataAccess
{
    [MetadataType(typeof(ScenarioFacultyRoleMetadata))]
    public class ScenarioFacultyRole
    {
        public Guid CourseId { get; set; }

        public Guid ScenarioId { get; set; }

        public Guid FacultyMemberId { get; set; }

        public Guid RoleId { get; set; }

        public virtual Course Course { get; set; }
        public virtual Scenario Scenario{ get; set; }
        public virtual ScenarioRoleDescription Role { get; set; }
        public virtual Participant FacultyMember { get; set; }
    }
}
