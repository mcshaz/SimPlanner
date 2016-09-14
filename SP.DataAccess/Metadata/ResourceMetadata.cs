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
        public string FileName { get; set; }
        [Range(0,250 * 1024)]
        public long? FileSize { get; set; }
	}
}
