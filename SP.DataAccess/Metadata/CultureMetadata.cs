using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Metadata
{
    public class CultureMetadata
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [FixedLength(Length = 5)]
        [RegularExpression("[a-z]{2}-[A-Z]{2}", ErrorMessage = "must be [2 lower case letters] [-(dash)] [2 upper case] - e.g. en-NZ")]
        public string LocaleCode { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        //[StringLength(3,MinimumLength = 2)]
        //[RegularExpression(@"\d+", ErrorMessage = "Culture dial code can only contain nubers")]
        [Range(1,999)]
        public int CountryCode { get; set; }
    }
}
