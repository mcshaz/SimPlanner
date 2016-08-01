using SP.DataAccess;
using SP.Dto;
using System;
using System.Linq.Expressions;
namespace SP.Dto.Maps
{
    internal static class CourseTypeMaps
    {
        internal static Func<CourseTypeDto, CourseType> MapToDomain()
        {
            return m => new CourseType
            {
                Id = m.Id,
                Abbreviation = m.Abbreviation,
                Description = m.Description,
                InstructorCourseId = m.InstructorCourseId, 
                EmersionCategory = m.EmersionCategory, 
            };
        }


        internal static Expression<Func<CourseType, CourseTypeDto>> MapFromDomain()
        {
            return m => new CourseTypeDto
            {
                Id = m.Id,
                Abbreviation = m.Abbreviation,
                Description = m.Description,
                InstructorCourseId = m.InstructorCourseId,
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
