using SP.DataAccess.Data.Interfaces;
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

        [Range(FileDefaults._minFileSize, FileDefaults._maxFileSize, ErrorMessage = FileDefaults._errMsg)]
        public long? FileSize { get; set; }
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
        [Range(FileDefaults._minFileSize, FileDefaults._maxFileSize, ErrorMessage = FileDefaults._errMsg)]
        [DefaultValue(0)]
        public long FileSize { get; set; }
        [Required]
        public DateTime FileModified { get; set; }
    }
}
