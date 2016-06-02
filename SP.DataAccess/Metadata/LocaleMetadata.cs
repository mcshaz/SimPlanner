using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SM.Metadata
{
    public class LocaleMetadata
    {
        [Key]
        [StringLength(5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RegularExpression("[A-Z]{2}-[a-z]{2}")]
        public string LocaleString { get; set; }
    }
}