using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.DTOs.Maps
{
    internal static class InstitutionMaps
    {        internal static Func<InstitutionDto, Institution> mapToRepo()
        {
            return m => new Institution
            {
                Id = m.Id,
                Name = m.Name,
                About = m.About,
                CountryCode = m.CountryCode
            };
        }


        internal static Expression<Func<Institution, InstitutionDto>> mapFromRepo()
        {
            return m => new InstitutionDto
            {
                Id = m.Id,
                Name = m.Name,
                About = m.About,
                CountryCode = m.CountryCode,
            };
        }
    }
}
