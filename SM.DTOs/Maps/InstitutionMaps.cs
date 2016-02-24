using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq;
using System.Linq.Expressions;
namespace SM.DTOs.Maps
{
    internal static class InstitutionMaps
    {        internal static Func<InstitutionDto, Institution> mapToRepo = m => new Institution
        {
            Id = m.Id,
            Name = m.Name,
            About = m.About,
            CountryCode = m.CountryCode
        };


        internal static Expression<Func<Institution, InstitutionDto>> mapFromRepo = m => new InstitutionDto
        {
            Id = m.Id,
            Name = m.Name,
            About = m.About,
            CountryCode = m.CountryCode,

            Country = new CountryDto
            {
                Code = m.Country.Code,
                Name = m.Country.Name,
                IsMonthFirst = m.Country.IsMonthFirst
            },
			
			Departments = m.Departments.Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                InstitutionId = d.InstitutionId,
                InvitationLetterFilename = d.InvitationLetterFilename,
                CertificateFilename = d.CertificateFilename
            })
        };
    }
}
