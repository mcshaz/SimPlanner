using SM.Metadata;
using System.ComponentModel.DataAnnotations;

namespace SM.DataAccess
{
    [MetadataType(typeof(CountryLocaleCodeMetadata))]
    public class CountryLocaleCode
    {
        public string LocaleCode { get; set; }
        public string CountryCode { get; set; }
        public byte Preference { get; set; }

        public virtual Country Country { get; set; }
    }
}
