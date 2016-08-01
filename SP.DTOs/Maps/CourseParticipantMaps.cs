using SP.DataAccess;
namespace SP.Dto.Maps
{
    internal class CourseParticipantMaps: DomainDtoMap<CourseParticipant, CourseParticipantDto>
    {
        public CourseParticipantMaps(): base(m => new CourseParticipant
            {
                ParticipantId = m.ParticipantId,
                CourseId = m.CourseId,
                IsConfirmed = m.IsConfirmed,
                IsFaculty = m.IsFaculty,
                IsOrganiser = m.IsOrganiser,
                DepartmentId = m.DepartmentId,
                ProfessionalRoleId = m.ProfessionalRoleId
                //Participant = m.Participant,
                //Course = m.Course
            }, 
            m => new CourseParticipantDto
            {
                ParticipantId = m.ParticipantId,
                CourseId = m.CourseId,
                IsConfirmed = m.IsConfirmed,
                IsFaculty = m.IsFaculty,
                IsOrganiser = m.IsOrganiser,
                DepartmentId = m.DepartmentId,
                ProfessionalRoleId = m.ProfessionalRoleId
                //Participant = m.Participant,
                //Course = m.Course
            })
        { }
    }
}
