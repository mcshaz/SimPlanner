using SP.DataAccess;

{
    internal class RoleMaps: DomainDtoMap<AspNetRole, RoleDto>
    {
        public RoleMaps() : base(m => new AspNetRole
            {
                Id = m.Id,
                Name =m.Name
            },
            m => new RoleDto
            {
                Id = m.Id,
                Name = m.Name
            })
        { }
    }
}