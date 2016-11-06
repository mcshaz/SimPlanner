using SP.DataAccess;
using SP.Dto.ProcessBreezeRequests;
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
            })
        {
            WherePredicate = v =>
            {
                if (v.AdminLevel == AdminLevels.AllData)
                {
                    return null;
                }
                return d => d.AdminApproved;
            };
        }
    }
}
