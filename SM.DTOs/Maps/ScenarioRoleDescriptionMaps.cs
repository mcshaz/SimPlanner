using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.Dto.Maps
{
    internal static class FacultySimRoleMaps
    {
        internal static Func<FacultySimRoleDto, FacultySimRole> mapToRepo()
        {
            return m => new FacultySimRole
            {
                Id = m.Id,
                Description = m.Description,
                //CourseTypes = m.CourseTypes,
                //ScenarioFacultyRoles = m.ScenarioFacultyRoles
            };
        }

        internal static Expression<Func<FacultySimRole, FacultySimRoleDto>> mapFromRepo()
        {
            return m => new FacultySimRoleDto
            {
                Id = m.Id,
                Description = m.Description,
                //CourseTypes = m.CourseTypes,
                //ScenarioFacultyRoles = m.ScenarioFacultyRoles
            };
        }
    }
}
