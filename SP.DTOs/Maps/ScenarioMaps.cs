using SP.DataAccess;
using SP.Dto;
using System;
using System.Linq.Expressions;
namespace SP.Dto.Maps
{
    internal static class ScenarioMaps
    {
        internal static Func<ScenarioDto, Scenario> mapToRepo()
        {
            return m => new Scenario
            {
                Id = m.Id,
                BriefDescription = m.BriefDescription,
                FullDescription =m.FullDescription,
                DepartmentId = m.DepartmentId,
                Complexity = m.Complexity,
                EmersionCategory = m.EmersionCategory,
                TemplateFilename = m.TemplateFilename,
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
                BriefDescription = m.BriefDescription,
                FullDescription = m.FullDescription,
                DepartmentId = m.DepartmentId,
                Complexity = m.Complexity,
                EmersionCategory = m.EmersionCategory,
                TemplateFilename = m.TemplateFilename,
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
