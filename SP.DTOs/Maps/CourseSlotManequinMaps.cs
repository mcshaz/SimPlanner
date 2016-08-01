using SP.DataAccess;
namespace SP.Dto.Maps
{
    internal class CourseSlotManequinMaps: DomainDtoMap<CourseSlotManequin, CourseSlotManequinDto>
    {
        public CourseSlotManequinMaps() : base(m => new CourseSlotManequin
            {
                CourseId = m.CourseId,
                CourseSlotId = m.CourseSlotId,
                ManequinId = m.ManequinId,
                StreamNumber = m.StreamNumber
                //Course = m.Course,
                //Scenario = m.Scenario,
                //Role = m.Role,
                //FacultyMember = m.FacultyMember
            },
            m => new CourseSlotManequinDto
            {
                CourseId = m.CourseId,
                CourseSlotId = m.CourseSlotId,
                ManequinId = m.ManequinId,
                StreamNumber = m.StreamNumber
                //Course = m.Course,
                //Scenario = m.Scenario,
                //Role = m.Role,
                //FacultyMember = m.FacultyMember
            })
        { }
    }
}
