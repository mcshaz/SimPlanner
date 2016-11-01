using SP.DataAccess;
using System;
using System.Linq.Expressions;

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
                DrinkPreferenceId = m.DrinkPreferenceId,
                DietNotes = m.DietNotes,
                UserName = m.UserName ?? m.Email,
                AdminApproved = m.AdminApproved
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
                DrinkPreferenceId = m.DrinkPreferenceId,
                DietNotes = m.DietNotes,
                UserName = m.UserName,
                AdminApproved = m.AdminApproved
                //CourseSlotPresentations = null,
                //Department = null,

                //ProfessionalRole = null,

                //CourseParticipants = null,

                //ScenarioFacultyRoles = null
            })
        {
            WherePredicate = v => (v.AdminLevel == ProcessBreezeRequests.AdminLevels.None)
                ? p => p.UserName == v.UserName
                : (Expression<Func<Participant,bool>>)null;
        }
    }
}
