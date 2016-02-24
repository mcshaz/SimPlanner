using SM.DataAccess;
using SM.Metadata;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;

namespace SM.Dto
{
    [MetadataType(typeof(CountryMetadata))]
    public class CountryDto
	{
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsMonthFirst { get; set; }
        public ICollection<InstitutionDto> Hospitals { get; set; }
        public ICollection<ProfessionalRoleDto> ProfessionalRoles { get; set; }

        internal static Func<CountryDto, Country> mapToRepo = m => new Country
        {
            Code = m.Code,
            Name = m.Name,
            IsMonthFirst = m.IsMonthFirst
            //Hospitals = m.Hospitals,
            //ProfessionalRoles = m.ProfessionalRoles
        };

        internal static Expression<Func<Country,CountryDto>> mapFromRepo = m => new CountryDto {
                Code = m.Code,
                Name = m.Name,
                IsMonthFirst = m.IsMonthFirst
                //Hospitals = m.Hospitals,
                //ProfessionalRoles = m.ProfessionalRoles
        };
    }
}
