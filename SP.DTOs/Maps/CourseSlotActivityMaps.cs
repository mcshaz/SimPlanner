using SP.DataAccess;

namespace SP.Dto.Maps
{
    internal class CourseSlotScenarioMaps: DomainDtoMap<CourseSlotActivity, CourseSlotActivityDto>
    {
        public CourseSlotScenarioMaps() : base(m => new CourseSlotActivity
            {
                CourseId = m.CourseId,
                CourseSlotId = m.CourseSlotId,
                ScenarioId = m.ScenarioId,
                ActivityId = m.ActivityId,
                StreamNumber = m.StreamNumber
                //Course = m.Course,
                //Scenario = m.Scenario,
                //Role = m.Role,
                //FacultyMember = m.FacultyMember
            },
            m => new CourseSlotActivityDto
            {
                CourseId = m.CourseId,
                CourseSlotId = m.CourseSlotId,
                ScenarioId = m.ScenarioId,
                ActivityId = m.ActivityId,
                StreamNumber = m.StreamNumber
                //Course = m.Course,
                //Scenario = m.Scenario,
                //Role = m.Role,
                //FacultyMember = m.FacultyMember
            })
        { }
    }
}
