using SM.DataAccess;
using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Linq;
namespace SM.Dto
{
    [MetadataType(typeof(InstitutionMetadata))]
    public class InstitutionDto
	{
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string About { get; set; }
        public string CountryCode { get; set; }
        public CountryDto Country { get; set; }
        public IEnumerable<DepartmentDto> Departments { get; set; }

        internal static Func<InstitutionDto, Institution> mapToRepo = m => new Institution
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
