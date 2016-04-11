using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SM.Dto
{
    [MetadataType(typeof(FacultySimRoleMetadata))]
    public class FacultySimRoleDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public Guid CourseTypeId { get; set; }
        public byte Order { get; set; }

        public virtual CourseTypeDto CourseType { get; set; }
        public virtual ICollection<CourseScenarioFacultyRoleDto> CourseScenarioFacultyRoles { get; set; }
    }
}
