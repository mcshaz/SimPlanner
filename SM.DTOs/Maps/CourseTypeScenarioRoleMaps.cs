using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.Dto.Maps
{
    internal static class CourseTypeScenarioRoleMaps
    {
        internal static Func<CourseTypeScenarioRoleDto, CourseTypeScenarioRole> mapToRepo()
        {
            return m => new CourseTypeScenarioRole
            {
                CourseTypeId = m.CourseTypeId,
                FacultyScenarioRoleId = m.FacultyScenarioRoleId
            };
        }


        internal static Expression<Func<CourseTypeScenarioRole, CourseTypeScenarioRoleDto>> mapFromRepo()
        {
            return m => new CourseTypeScenarioRoleDto
            {
                CourseTypeId = m.CourseTypeId,
                FacultyScenarioRoleId = m.FacultyScenarioRoleId
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
