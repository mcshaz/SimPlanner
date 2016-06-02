using SM.Metadata;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SM.DataAccess
{
    [MetadataType(typeof(LocaleMetadata))]
    public class Locale
    {
        public string LocaleString { get; set; }

        ICollection<CountryLocale> _countryLocales;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CountryLocale> CountryLocales
        {
            get
            {
                return _countryLocales ?? (_countryLocales = new List<CountryLocale>());
            }
            set
            {
                _countryLocales = value;
            }
        }

    }
}
