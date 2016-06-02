using SM.Metadata;
using System.ComponentModel.DataAnnotations;

namespace SM.Dto
{
    [MetadataType(typeof(CountryLocaleCodeMetadata))]
    public class CountryLocaleCodeDto
    {
        public string LocaleCode { get; set; }
        public string CountryCode { get; set; }
        public byte Preference { get; set; }

        public virtual CountryDto Country { get; set; }
    }
}
