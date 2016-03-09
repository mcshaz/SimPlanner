namespace SM.DataAccess
{
    using SM.Metadata;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(CountryMetadata))]
    public class Country
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string DialCode { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Institution> Hospitals { get; set; }

        ICollection<CountryLocaleCode> _countryLocales;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CountryLocaleCode> CountryLocales
        {
            get
            {
                return _countryLocales ?? (_countryLocales = new List<CountryLocaleCode>());
            }
            set
            {
                _countryLocales = value;
            }
        }
    }
}
