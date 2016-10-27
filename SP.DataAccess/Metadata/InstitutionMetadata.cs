using SP.DataAccess.Data.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Metadata
{
    public class InstitutionMetadata
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(20)]
        public string Abbreviation { get; set; }
        //[AllowHtml]
        //public string About { get; set; }
        [FixedLength(Length=5)]
        public string LocaleCode { get; set; }

        [Range(FileDefaults._minFileSize, FileDefaults._maxFileSize, ErrorMessage = FileDefaults._errMsg)]
        public long? FileSize { get; set; }

        [StringLength(40)]
        public string StandardTimeZone { get; set; }
    }
}
