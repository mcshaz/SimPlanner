using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SM.Metadata
{
    public class CountryLocaleCodeMetadata
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string CountryCode { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [FixedLength(Length=5)]
        [RegularExpression("[a-z]{2}-[A-Z]{2}", ErrorMessage = "must be [2 lower case letters] [-(dash)] [2 upper case] - e.g. en-NZ")]
        public string LocaleCode { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte Preference { get; set; }
    }
}
