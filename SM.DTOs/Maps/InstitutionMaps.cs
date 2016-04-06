using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.Dto.Maps
{
    internal static class InstitutionMaps
    {        internal static Func<InstitutionDto, Institution> mapToRepo()
        {
            return m => new Institution
            {
                Id = m.Id,
                Name = m.Name,
                About = m.About,
                LocaleCode = m.LocaleCode,
                StandardTimeZone = m.StandardTimeZone
            };
        }


        internal static Expression<Func<Institution, InstitutionDto>> mapFromRepo()
        {
            return m => new InstitutionDto
            {
                Id = m.Id,
                Name = m.Name,
                About = m.About,
                LocaleCode = m.LocaleCode,
                StandardTimeZone = m.StandardTimeZone
            };
        }
    }
}
