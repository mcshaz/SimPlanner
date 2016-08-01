using SP.DataAccess;
namespace SP.Dto.Maps
{
    internal class FacultyScenarioRoleMaps: DomainDtoMap<FacultyScenarioRole, FacultyScenarioRoleDto>
    {
        public FacultyScenarioRoleMaps() : base(m => new FacultyScenarioRole
            {
                Id = m.Id,
                Description = m.Description,
                Order = m.Order
                //CourseTypes = m.CourseTypes,
                //ScenarioFacultyRoles = m.ScenarioFacultyRoles
            },
            m => new FacultyScenarioRoleDto
            {
                Id = m.Id,
                Description = m.Description,
                Order = m.Order
                //CourseTypes = m.CourseTypes,
                //ScenarioFacultyRoles = m.ScenarioFacultyRoles
            })
        { }
    }
}
