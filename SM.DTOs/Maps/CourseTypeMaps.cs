using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.Dto.Maps
{
    internal static class CourseTypeMaps
    {
        internal static Func<CourseTypeDto, CourseType> mapToRepo()
        {
            return m => new CourseType
            {
                Id = m.Id,
                Abbrev = m.Abbrev,
                Description = m.Description,
                IsInstructorCourse = m.IsInstructorCourse, 
                EmersionCategory = m.EmersionCategory, 
            };
        }


        internal static Expression<Func<CourseType, CourseTypeDto>> mapFromRepo()
        {
            return m => new CourseTypeDto
            {
                Id = m.Id,
                Abbrev = m.Abbrev,
                Description = m.Description,
                IsInstructorCourse = m.IsInstructorCourse,
                EmersionCategory = m.EmersionCategory,
                //Departments = m.Departments,

                //Scenarios = m.Scenarios,

                //Courses = m.Courses,

                //CourseEvents = m.CourseEvents,

                //ScenarioEvents = m.ScenarioEvents,

                //ScenarioRoles = m.ScenarioRoles
            };
        }
    }
}
