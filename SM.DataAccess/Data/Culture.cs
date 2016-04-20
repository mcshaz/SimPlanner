namespace SM.DataAccess
{
    using SM.Metadata;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(CultureMetadata))]
    public class Culture
    {
        public string LocaleCode { get; set; }

        public string Name { get; set; }

        public int CountryCode { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Institution> Institutions { get; set; }

    }
}
