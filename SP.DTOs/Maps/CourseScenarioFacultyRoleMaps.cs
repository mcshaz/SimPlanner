using SP.DataAccess;

namespace SP.Dto.Maps
{
    internal class CourseScenarioFacultyRoleMaps: DomainDtoMap<CourseScenarioFacultyRole, CourseScenarioFacultyRoleDto>
    {
        public CourseScenarioFacultyRoleMaps() : base(
            m => new CourseScenarioFacultyRole
            {
                CourseId = m.CourseId,
                CourseSlotId = m.CourseSlotId,
                ParticipantId = m.ParticipantId,
                FacultyScenarioRoleId = m.FacultyScenarioRoleId
            },
            m => new CourseScenarioFacultyRoleDto
            {
                CourseId = m.CourseId,
                CourseSlotId = m.CourseSlotId,
                ParticipantId = m.ParticipantId,
                FacultyScenarioRoleId = m.FacultyScenarioRoleId
            })
        { }
    }
}
