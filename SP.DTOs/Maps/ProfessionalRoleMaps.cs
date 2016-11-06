using SP.DataAccess;

namespace SP.Dto.Maps
{
    internal class ProfessionalRoleMaps : DomainDtoMap<ProfessionalRole, ProfessionalRoleDto>
    {
        public ProfessionalRoleMaps() : base(m => new ProfessionalRole
        {
                Id = m.Id,
                Description = m.Description,
                Category = m.Category,
                Order = m.Order
            },
            m => new ProfessionalRoleDto
            {
                Id = m.Id,
                Description = m.Description,
                Category = m.Category,
                Order = m.Order
            })
        { }
    }
}
