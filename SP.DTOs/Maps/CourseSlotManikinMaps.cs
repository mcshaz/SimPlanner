using SP.DataAccess;
namespace SP.Dto.Maps
{
    internal class CourseSlotManikinMaps: DomainDtoMap<CourseSlotManikin, CourseSlotManikinDto>
    {
        public CourseSlotManikinMaps() : base(m => new CourseSlotManikin
            {
                CourseId = m.CourseId,
                CourseSlotId = m.CourseSlotId,
                ManikinId = m.ManikinId,
                StreamNumber = m.StreamNumber
                //Course = m.Course,
                //Scenario = m.Scenario,
                //Role = m.Role,
                //FacultyMember = m.FacultyMember
            },
            m => new CourseSlotManikinDto
            {
                CourseId = m.CourseId,
                CourseSlotId = m.CourseSlotId,
                ManikinId = m.ManikinId,
                StreamNumber = m.StreamNumber
                //Course = m.Course,
                //Scenario = m.Scenario,
                //Role = m.Role,
                //FacultyMember = m.FacultyMember
            })
        { }
    }
}
