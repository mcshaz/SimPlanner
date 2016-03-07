using SM.Metadata;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SM.Dto
{
    [MetadataType(typeof(LocaleMetadata))]
    public class LocaleDto
    {
        public string LocaleString { get; set; }

        public virtual ICollection<CountryLocaleDto> CountryLocales { get; set; }
    }
}
