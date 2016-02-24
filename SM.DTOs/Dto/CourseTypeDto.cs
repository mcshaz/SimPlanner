using SM.DataAccess.Enums;
using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SM.Dto
{
    [MetadataType(typeof(CourseTypeMetadata))]
    public class CourseTypeDto
	{
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool IsInstructorCourse { get; set; }
        public byte DaysDuration { get; set; }
        public Emersion? EmersionCategory { get; set; }
        public string Abbrev { get; set; }
        public ICollection<DepartmentDto> Departments { get; set; }
        public ICollection<ScenarioDto> Scenarios { get; set; }
        public ICollection<CourseDto> Courses { get; set; }
        public ICollection<CourseSlotDto> CourseEvents { get; set; }
        public ICollection<ScenarioSlotDto> ScenarioEvents { get; set; }
        public ICollection<ScenarioRoleDescriptionDto> ScenarioRoles { get; set; }
    }
}
