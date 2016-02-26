using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SM.Metadata
{        
	public class InstitutionMetadata
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        //[AllowHtml]
        //public string About { get; set; }
        [FixedLength(Length=2)]
        public string CountryCode { get; set; }
    }
}
