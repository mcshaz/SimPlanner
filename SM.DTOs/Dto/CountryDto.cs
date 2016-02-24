using SM.Metadata;
using System.Collections.Generic;
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

    }
}
