namespace SP.DataAccess
{
    using SP.Metadata;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    [MetadataType(typeof(CultureMetadata))]
    public class Culture
    {
        public string LocaleCode { get; set; }

        public string Name { get; set; }

        public int CountryCode { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Institution> Institutions { get; set; }

    }

    public static class CultureExtensions
    {
        public static CultureInfo GetCultureInfo(this Culture culture)
        {
            return CultureInfo.GetCultureInfo(culture.LocaleCode);
        }
    }
}
