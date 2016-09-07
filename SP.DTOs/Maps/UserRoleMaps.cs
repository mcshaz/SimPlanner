using SP.DataAccess;

namespace SP.Dto.Maps
{
    internal class UserRoleMaps : DomainDtoMap<AspNetUserRole, UserRoleDto>
    {
        public UserRoleMaps() : base(m => new AspNetUserRole
            {
                UserId = m.UserId,
                RoleId = m.RoleId
            },
            m => new UserRoleDto
            {
                UserId = m.UserId,
                RoleId = m.RoleId
            })
        { }
    }
}
