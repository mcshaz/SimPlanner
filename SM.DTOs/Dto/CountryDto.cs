using SM.Metadata;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SM.Dto
{
    [MetadataType(typeof(CountryMetadata))]
    public class CountryDto
	{
        public string LocaleCode { get; set; }
        public string Name { get; set; }
        public string DialCode { get; set; }
        public virtual ICollection<InstitutionDto> Institutions { get; set; }
    }
}
