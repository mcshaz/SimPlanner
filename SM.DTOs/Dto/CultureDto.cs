using SM.Metadata;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SM.Dto
{
    [MetadataType(typeof(CultureMetadata))]
    public class CultureDto
	{
        public string LocaleCode { get; set; }
        public string Name { get; set; }
        public int CountryCode { get; set; }
        public virtual ICollection<InstitutionDto> Institutions { get; set; }
    }
}
