using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SM.Dto
{
    [MetadataType(typeof(ScenarioRoleDescriptionMetadata))]
    public class ScenarioRoleDescriptionDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public ICollection<CourseTypeDto> CourseTypes { get; set; }
        public ICollection<ScenarioFacultyRoleDto> ScenarioFacultyRoles { get; set; }

    }
}
