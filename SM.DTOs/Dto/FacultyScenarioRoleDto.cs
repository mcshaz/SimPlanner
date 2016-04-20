using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SM.Dto
{
    [MetadataType(typeof(FacultyScenarioRoleMetadata))]
    public class FacultyScenarioRoleDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }

        public virtual ICollection<CourseScenarioFacultyRoleDto> CourseScenarioFacultyRoles { get; set; }
        public virtual ICollection<CourseTypeScenarioRoleDto> CourseTypeScenarioRoles { get; set; }
    }
}
