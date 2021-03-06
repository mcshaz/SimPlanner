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
        public string DialCode { get; set; }

        public virtual ICollection<Institution> Institutions { get; set; }

        CultureInfo _cultureInfo;
        public virtual CultureInfo CultureInfo
        {
            get
            {
                return _cultureInfo ?? (_cultureInfo = CultureInfo.GetCultureInfo(LocaleCode));
            }
        }
    }
}
