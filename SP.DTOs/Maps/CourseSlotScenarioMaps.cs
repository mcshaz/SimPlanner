using SP.DataAccess;
namespace SP.Dto.Maps
{
    internal class CourseSlotScenarioMaps: DomainDtoMap<CourseSlotScenario, CourseSlotScenarioDto>
    {
        public CourseSlotScenarioMaps() : base(m => new CourseSlotScenario
            {
                CourseId = m.CourseId,
                CourseSlotId = m.CourseSlotId,
                ScenarioId = m.ScenarioId,
                StreamNumber = m.StreamNumber
                //Course = m.Course,
                //Scenario = m.Scenario,
                //Role = m.Role,
                //FacultyMember = m.FacultyMember
            },
            m => new CourseSlotScenarioDto
            {
                CourseId = m.CourseId,
                CourseSlotId = m.CourseSlotId,
                ScenarioId = m.ScenarioId,
                StreamNumber = m.StreamNumber
                //Course = m.Course,
                //Scenario = m.Scenario,
                //Role = m.Role,
                //FacultyMember = m.FacultyMember
            })
        { }
    }
}
