using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.DTOs.Maps
{
    internal static class ScenarioRoleDescriptionMaps
    {
        internal static Func<ScenarioRoleDescriptionDto, ScenarioRoleDescription> mapToRepo()
        {
            return m => new ScenarioRoleDescription
            {
                Id = m.Id,
                Description = m.Description,
                //CourseTypes = m.CourseTypes,
                //ScenarioFacultyRoles = m.ScenarioFacultyRoles
            };
        }

        internal static Expression<Func<ScenarioRoleDescription, ScenarioRoleDescriptionDto>> mapFromRepo()
        {
            return m => new ScenarioRoleDescriptionDto
            {
                Id = m.Id,
                Description = m.Description,
                //CourseTypes = m.CourseTypes,
                //ScenarioFacultyRoles = m.ScenarioFacultyRoles
            };
        }
    }
}
