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
                ProfessionalRoleId = m.ProfessionalRoleId,
                EmailTimeStamp = m.EmailTimeStamp,
                CreatedUtc = m.Created,
                LastModifiedUtc = m.LastModified
            }, 
            m => new CourseParticipantDto
            {
                ParticipantId = m.ParticipantId,
                CourseId = m.CourseId,
                IsConfirmed = m.IsConfirmed,
                IsFaculty = m.IsFaculty,
                IsOrganiser = m.IsOrganiser,
                DepartmentId = m.DepartmentId,
                ProfessionalRoleId = m.ProfessionalRoleId,
                EmailTimeStamp = m.EmailTimeStamp,
                Created = m.CreatedUtc,
                LastModified = m.LastModifiedUtc
            })
        { }
    }
}
