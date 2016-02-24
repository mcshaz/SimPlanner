using SM.DataAccess;
using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
namespace SM.Dto
{
    [MetadataType(typeof(ScenarioRoleDescriptionMetadata))]
    public class ScenarioRoleDescriptionDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public ICollection<CourseTypeDto> CourseTypes { get; set; }
        public ICollection<ScenarioFacultyRoleDto> ScenarioFacultyRoles { get; set; }

        internal static Func<ScenarioRoleDescriptionDto, ScenarioRoleDescription>  mapToRepo = m => new ScenarioRoleDescription {
            Id = m.Id,
            Description = m.Description,
            //CourseTypes = m.CourseTypes,
            //ScenarioFacultyRoles = m.ScenarioFacultyRoles
        };

        internal static Expression<Func<ScenarioRoleDescription, ScenarioRoleDescriptionDto>> mapFromRepo= m => new ScenarioRoleDescriptionDto
        {
            Id = m.Id,
            Description = m.Description,
            //CourseTypes = m.CourseTypes,
            //ScenarioFacultyRoles = m.ScenarioFacultyRoles
        };
    }
}
