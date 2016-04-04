namespace SM.DataAccess
{
    using SM.Metadata;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(CountryMetadata))]
    public class Country
    {
        public string LocaleCode { get; set; }

        public string Name { get; set; }

        public string DialCode { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Institution> Institutions { get; set; }

    }
}
