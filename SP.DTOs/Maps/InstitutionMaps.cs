using SP.DataAccess;
using SP.Dto;
using System;
using System.Linq.Expressions;
namespace SP.Dto.Maps
{
    internal static class InstitutionMaps
    {        internal static Func<InstitutionDto, Institution> MapToDomain()
        {
            return m => new Institution
            {
                Id = m.Id,
                Name = m.Name,
                About = m.About,
                LocaleCode = m.LocaleCode,
                StandardTimeZone = m.StandardTimeZone,
                Latitude = m.Latitude,
                Longitude = m.Longitude
            };
        }


        internal static Expression<Func<Institution, InstitutionDto>> MapFromDomain()
        {
            return m => new InstitutionDto
            {
                Id = m.Id,
                Name = m.Name,
                About = m.About,
                LocaleCode = m.LocaleCode,
                StandardTimeZone = m.StandardTimeZone,
                Latitude = m.Latitude,
                Longitude = m.Longitude
            };
        }
    }
}
