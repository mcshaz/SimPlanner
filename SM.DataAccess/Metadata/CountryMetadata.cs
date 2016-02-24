using System.ComponentModel.DataAnnotations;

namespace SM.Metadata
{    
    public class CountryMetadata 
	{
        [Key]
        [StringLength(2)]
        public string Code { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
