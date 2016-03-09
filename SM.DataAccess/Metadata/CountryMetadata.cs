using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SM.Metadata
{
    public class CountryMetadata
    {
        [Key]
        [FixedLength(Length = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Code { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(3,MinimumLength = 2)]
        [RegularExpression(@"\d+", ErrorMessage = "country dial code can only contain nubers")]
        public string DialCode { get; set; }
    }
}
