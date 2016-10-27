using SP.DataAccess;

namespace SP.Dto.Maps
{
    internal class InstitutionMaps: DomainDtoMap<Institution, InstitutionDto>
    {
        public InstitutionMaps() : base(m => new Institution
            {
                Id = m.Id,
                Name = m.Name,
                About = m.About,
                Abbreviation = m.Abbreviation,
                LocaleCode = m.LocaleCode,
                StandardTimeZone = m.StandardTimeZone,
                Latitude = m.Latitude,
                Longitude = m.Longitude,
                AdminApproved = m.AdminApproved,
                FileModified = m.FileModified,
                LogoImageFileName = m.LogoImageFileName,
                FileSize = m.FileSize
            },
            m => new InstitutionDto
            {
                Id = m.Id,
                Name = m.Name,
                About = m.About,
                Abbreviation = m.Abbreviation,
                LocaleCode = m.LocaleCode,
                StandardTimeZone = m.StandardTimeZone,
                Latitude = m.Latitude,
                Longitude = m.Longitude,
                AdminApproved = m.AdminApproved,
                FileModified = m.FileModified,
                LogoImageFileName = m.LogoImageFileName,
                FileSize = m.FileSize
            })
        { }
    }
}
