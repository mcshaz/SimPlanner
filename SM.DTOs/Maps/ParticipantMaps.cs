using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;

namespace SM.DTOs.Maps
{
    public static class ParticipantMaps
    {
        public static Func<ParticipantDto, Participant> mapToRepo = m => new Participant
        {
            Id = m.Id,
            PhoneNumber = m.PhoneNumber,
            Email = m.Email,
            AlternateEmail = m.AlternateEmail,
            FullName = m.FullName,
            DefaultDepartmentId = m.DefaultDepartmentId,
            DefaultProfessionalRoleId = m.DefaultProfessionalRoleId,

            //Department = m.Department,

            //ProfessionalRole = m.ProfessionalRole,

            //CourseParticipants = m.CourseParticipants,

            //ScenarioFacultyRoles = m.ScenarioFacultyRoles
        };

        public static Expression<Func<Participant, ParticipantDto>> mapFromRepo = m => new ParticipantDto
        {
            Id = m.Id,
            PhoneNumber = m.PhoneNumber,
            Email = m.Email,
            AlternateEmail = m.AlternateEmail,
            FullName = m.FullName,
            DefaultDepartmentId = m.DefaultDepartmentId,
            DefaultProfessionalRoleId = m.DefaultProfessionalRoleId,

            //CourseSlotPresentations = null,
            Department = null,

            //ProfessionalRole = null,

            //CourseParticipants = null,

            //ScenarioFacultyRoles = null
        };
    }
}
