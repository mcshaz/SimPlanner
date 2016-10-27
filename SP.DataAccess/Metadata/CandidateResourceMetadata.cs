using SP.DataAccess.Data.Interfaces;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Metadata
{
	public class CandidatePrereadingMetadata
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [StringLength(256)]
        [Required]
        public string FileName { get; set; }
        [Range(FileDefaults._minFileSize, FileDefaults._maxFileSize, ErrorMessage = FileDefaults._errMsg)]
        public long FileSize { get; set; }
        public DateTime FileModified { get; set; }
    }
}
