using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.Dto.Maps
{
    internal static class ScenarioMaps
    {
        internal static Func<ScenarioDto, Scenario> mapToRepo()
        {
            return m => new Scenario
            {
                Id = m.Id,
                Description = m.Description,
                DepartmentId = m.DepartmentId,
                Complexity = m.Complexity,
                EmersionCategory = m.EmersionCategory,
                TemplateFilename = m.TemplateFilename,
                ManequinId = m.ManequinId,
                CourseTypeId = m.CourseTypeId,
                //Manequin = m.Manequin,
                //CourseType = m.CourseType,
                //Department = m.Department,
                //Courses = m.Courses,
                //Resources = m.Resources,
                //ScenarioFacultyRoles = m.ScenarioFacultyRoles
            };
        }

        internal static Expression<Func<Scenario, ScenarioDto>> mapFromRepo()
        {
            return m => new ScenarioDto
            {
                Id = m.Id,
                Description = m.Description,
                DepartmentId = m.DepartmentId,
                Complexity = m.Complexity,
                EmersionCategory = m.EmersionCategory,
                TemplateFilename = m.TemplateFilename,
                ManequinId = m.ManequinId,
                CourseTypeId = m.CourseTypeId,
                //Manequin = m.Manequin,
                //CourseType = m.CourseType,
                //Department = m.Department,
                //Courses = m.Courses,
                //Resources = m.Resources,
                //ScenarioFacultyRoles = m.ScenarioFacultyRoles
            };
        }
    }
}
