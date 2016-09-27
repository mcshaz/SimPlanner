using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Metadata
{        
	public class CourseFormatMetadata
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [Required(AllowEmptyStrings = true)]
        [StringLength(50)]
        public string Description { get; set; }
        [Range(1,250)]
        [DefaultValue(1)]
        public byte DaysDuration { get; set; }
        [Required]
        [DefaultValue(typeof(TimeSpan),"08:00")]
        public TimeSpan DefaultStartTime { get; set; }
    }
}
