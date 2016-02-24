using SM.DataAccess;
using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
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
        public ICollection<ScenarioRoleDescription> ScenarioRoles { get; set; }


        internal static Func<CourseTypeDto, CourseType> mapToRepo = m => new CourseType
        {
            Id = m.Id,
            Description = m.Description,
            IsInstructorCourse = m.IsInstructorCourse,
            DaysDuration = m.DaysDuration,
            EmersionCategory = m.EmersionCategory,
            Abbrev = m.Abbrev
        };


        internal static Expression<Func<CourseType, CourseTypeDto>> mapFromRepo= m => new CourseTypeDto
        {
            Id = m.Id,
            Description = m.Description,
            IsInstructorCourse = m.IsInstructorCourse,
            DaysDuration = m.DaysDuration,
            EmersionCategory = m.EmersionCategory,
            Abbrev = m.Abbrev

            //Departments = m.Departments,

            //Scenarios = m.Scenarios,

            //Courses = m.Courses,

            //CourseEvents = m.CourseEvents,

            //ScenarioEvents = m.ScenarioEvents,

            //ScenarioRoles = m.ScenarioRoles
        };
    }
}
