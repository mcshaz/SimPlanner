using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.Dto.Maps
{
    internal static class ScenarioFacultyRoleMaps
    {
        internal static Func<ScenarioFacultyRoleDto, ScenarioFacultyRole> mapToRepo()
        { 
            return m => new ScenarioFacultyRole {
                CourseId = m.CourseId,
                ScenarioId = m.ScenarioId,
                FacultyMemberId = m.FacultyMemberId,
                RoleId = m.RoleId,
                //Course = m.Course,
                //Scenario = m.Scenario,
                //Role = m.Role,
                //FacultyMember = m.FacultyMember
            };
        }

        internal static Expression<Func<ScenarioFacultyRole, ScenarioFacultyRoleDto>> mapFromRepo()
        {
            return m => new ScenarioFacultyRoleDto
            {
                CourseId = m.CourseId,
                ScenarioId = m.ScenarioId,
                FacultyMemberId = m.FacultyMemberId,
                RoleId = m.RoleId,
                //Course = m.Course,
                //Scenario = m.Scenario,
                //Role = m.Role,
                //FacultyMember = m.FacultyMember
            };
        }
    }
}
