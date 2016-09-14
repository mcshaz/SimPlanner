using SP.DataAccess;

namespace SP.Dto.Maps
{
    internal class ScenarioMaps: DomainDtoMap<Scenario, ScenarioDto>
    {
        public ScenarioMaps() : base(m => new Scenario
            {
                Id = m.Id,
                BriefDescription = m.BriefDescription,
                FullDescription =m.FullDescription,
                DepartmentId = m.DepartmentId,
                Complexity = m.Complexity,
                EmersionCategory = m.EmersionCategory,
                CourseTypeId = m.CourseTypeId,
                Access = m.Access
                //Manequin = m.Manequin,
                //CourseType = m.CourseType,
                //Department = m.Department,
                //Courses = m.Courses,
                //Resources = m.Resources,
                //ScenarioFacultyRoles = m.ScenarioFacultyRoles
            },
            m => new ScenarioDto
            {
                Id = m.Id,
                BriefDescription = m.BriefDescription,
                FullDescription = m.FullDescription,
                DepartmentId = m.DepartmentId,
                Complexity = m.Complexity,
                EmersionCategory = m.EmersionCategory,
                CourseTypeId = m.CourseTypeId,
                Access = m.Access
                //Manequin = m.Manequin,
                //CourseType = m.CourseType,
                //Department = m.Department,
                //Courses = m.Courses,
                //Resources = m.Resources,
                //ScenarioFacultyRoles = m.ScenarioFacultyRoles
            })
        { }
    }
}
