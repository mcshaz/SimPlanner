using SP.DataAccess;
using SP.Dto;
using System;
using System.Linq.Expressions;

namespace SP.Dto.Maps
{
    public static class ParticipantMaps
    {
        public static Func<ParticipantDto, Participant> MapToDomain()
        {
            return m => new Participant
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
            };
        }

        public static Expression<Func<Participant, ParticipantDto>> MapFromDomain()
        {
            return m => new ParticipantDto
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
            };
        }
    }
}
