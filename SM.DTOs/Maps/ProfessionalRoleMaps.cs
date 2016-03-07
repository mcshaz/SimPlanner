using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.DTOs.Maps
{
    internal static class ProfessionalRoleMaps
    {
        internal static Func<ProfessionalRoleDto, ProfessionalRole> mapToRepo()
        {
            return m => new ProfessionalRole
            {
                Id = m.Id,
                Description = m.Description,
                Category = m.Category,
                //Participants = m.Participants,
                //Countries = m.Countries
            };
        }

        internal static Expression<Func<ProfessionalRole, ProfessionalRoleDto>> mapFromRepo()
        {
            return m => new ProfessionalRoleDto
            {
                Id = m.Id,
                Description = m.Description,
                Category = m.Category,
                //Participants = m.Participants,
                //Countries = m.Countries
            };
        }
    }
}
