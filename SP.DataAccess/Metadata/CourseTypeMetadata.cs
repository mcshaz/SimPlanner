using SP.DataAccess.Data.Interfaces;
using SP.DataAccess.Metadata.Attributes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Metadata
{        
	public class CourseTypeMetadata
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Description { get; set; }
        [Required]
        [StringLength(32)]
        public string Abbreviation { get; set; }

        [Range(FileDefaults._minFileSize, FileDefaults._maxFileSize, ErrorMessage = FileDefaults._errMsg)]
        public long? FileSize { get; set; }

        //[Required]
        //[DefaultValue("Generic Certificate Template.pptx")]
        [ValidFileName(AllowedExtensions = ".pptx")]
        public string CertificateFileName { get; set; }
    }
}
