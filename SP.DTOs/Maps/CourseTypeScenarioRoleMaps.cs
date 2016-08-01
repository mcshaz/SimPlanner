using SP.DataAccess;
namespace SP.Dto.Maps
{
    internal class CourseTypeScenarioRoleMaps: DomainDtoMap<CourseTypeScenarioRole, CourseTypeScenarioRoleDto>
    {
        public CourseTypeScenarioRoleMaps() : base(m => new CourseTypeScenarioRole
            {
                CourseTypeId = m.CourseTypeId,
                FacultyScenarioRoleId = m.FacultyScenarioRoleId
            },
            m => new CourseTypeScenarioRoleDto
            {
                CourseTypeId = m.CourseTypeId,
                FacultyScenarioRoleId = m.FacultyScenarioRoleId
                //Departments = m.Departments,

                //Scenarios = m.Scenarios,

                //Courses = m.Courses,

                //CourseEvents = m.CourseEvents,

                //ScenarioEvents = m.ScenarioEvents,

                //ScenarioRoles = m.ScenarioRoles
            })
        { }
    }
}
