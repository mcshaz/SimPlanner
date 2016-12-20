using SP.DataAccess;
using SP.Dto.ProcessBreezeRequests;

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
            HomepageUrl = m.HomepageUrl,
            FileModified = m.FileModified,
            LogoImageFileName = m.LogoImageFileName,
            FileSize = m.FileSize,
            File = m.File
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
                HomepageUrl = m.HomepageUrl,
                FileModified = m.FileModified,
                LogoImageFileName = m.LogoImageFileName,
                FileSize = m.FileSize
            })
        {
            WherePredicate = v =>
            {
                if (v.AdminLevel == AdminLevels.AllData || v.AdminLevel == AdminLevels.None)
                {
                    return null;
                }
                return i => i.AdminApproved;
            };
        }
    }
}
