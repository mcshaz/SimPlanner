using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.DTOs.Maps
{
    internal static class CourseTypeMaps
    {
        internal static Func<CourseTypeDto, CourseType> mapToRepo()
        {
            return m => new CourseType
            {
                Id = m.Id,
                Description = m.Description,
                IsInstructorCourse = m.IsInstructorCourse,
                DaysDuration = m.DaysDuration,
                EmersionCategory = m.EmersionCategory,
                Abbrev = m.Abbrev
            };
        }


        internal static Expression<Func<CourseType, CourseTypeDto>> mapFromRepo()
        {
            return m => new CourseTypeDto
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
}
