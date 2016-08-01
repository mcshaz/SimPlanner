using SP.DataAccess;namespace SP.Dto.Maps
{
    internal class ProfessionalRoleInstitutionMaps: DomainDtoMap<ProfessionalRoleInstitution, ProfessionalRoleInstitutionDto>
    {
        public ProfessionalRoleInstitutionMaps() : base(m => new ProfessionalRoleInstitution
        {
            InstitutionId = m.InstitutionId,
            ProfessionalRoleId = m.ProfessionalRoleId
        },
        m => new ProfessionalRoleInstitutionDto
        {
            InstitutionId = m.InstitutionId,
            ProfessionalRoleId = m.ProfessionalRoleId
        })
        { }
    }
}
