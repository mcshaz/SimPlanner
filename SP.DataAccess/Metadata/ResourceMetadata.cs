using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Metadata
{
	public class ActivityResourceMetadata
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [Required]
        [StringLength(128)]
        public string Description { get; set; }
        [StringLength(256)]
        public string FileName { get; set; }
        internal const int _maxFileSize = 250 * 1024;
        [Range(0,_maxFileSize, ErrorMessage = "File Size must be less than 250 KB")]
        [DefaultValue(0)]
        public long FileSize { get; set; }
        public DateTime? FileModified { get; set; }
    }

    public class ScenarioResourceMetadata
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [Required]
        [StringLength(128)]
        public string Description { get; set; }
        [StringLength(256)]
        [Required]
        public string FileName { get; set; }
        [Required]
        [Range(0, ActivityResourceMetadata._maxFileSize, ErrorMessage = "File Size must be less than 250 KB")]
        [DefaultValue(0)]
        public long FileSize { get; set; }
        [Required]
        public DateTime? FileModified { get; set; }
    }
}
