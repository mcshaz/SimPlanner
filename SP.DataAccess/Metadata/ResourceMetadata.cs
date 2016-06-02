using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Metadata
{       
	public class ResourceMetadata
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [Required]
        [StringLength(128)]
        public string Description { get; set; }
        [StringLength(256)]
        public string ResourceFilename { get; set; }
	}
}
