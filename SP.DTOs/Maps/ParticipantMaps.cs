using SP.DataAccess;

namespace SP.Dto.Maps
{
    internal class ParticipantMaps: DomainDtoMap<Participant, ParticipantDto>
    {
        public ParticipantMaps() : base(m => new Participant
            {
                Id = m.Id,
                PhoneNumber = m.PhoneNumber,
                Email = m.Email,
                AlternateEmail = m.AlternateEmail,
                FullName = m.FullName,
                DefaultDepartmentId = m.DefaultDepartmentId,
                DefaultProfessionalRoleId = m.DefaultProfessionalRoleId,
                UserName = m.UserName
                //Department = m.Department,

                //ProfessionalRole = m.ProfessionalRole,

                //CourseParticipants = m.CourseParticipants,

                //ScenarioFacultyRoles = m.ScenarioFacultyRoles
            }, m => new ParticipantDto
            {
                Id = m.Id,
                PhoneNumber = m.PhoneNumber,
                Email = m.Email,
                AlternateEmail = m.AlternateEmail,
                FullName = m.FullName,
                DefaultDepartmentId = m.DefaultDepartmentId,
                DefaultProfessionalRoleId = m.DefaultProfessionalRoleId,
                UserName = m.UserName
                //CourseSlotPresentations = null,
                //Department = null,

                //ProfessionalRole = null,

                //CourseParticipants = null,

                //ScenarioFacultyRoles = null
            })
        { }
    }
}
