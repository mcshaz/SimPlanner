using SP.DataAccess;
using SP.Dto;
using System;
using System.Linq.Expressions;
namespace SP.Dto.Maps
{
    internal static class ProfessionalRoleMaps
    {
        internal static Func<ProfessionalRoleDto, ProfessionalRole> MapToDomain()
        {
            return m => new ProfessionalRole
            {
                Id = m.Id,
                Description = m.Description,
                Category = m.Category,
                Order = m.Order
                //Participants = m.Participants,
                //cultures = m.cultures
            };
        }

        internal static Expression<Func<ProfessionalRole, ProfessionalRoleDto>> MapFromDomain()
        {
            return m => new ProfessionalRoleDto
            {
                Id = m.Id,
                Description = m.Description,
                Category = m.Category,
                Order = m.Order
                //Participants = m.Participants,
                //cultures = m.cultures
            };
        }
    }
}
