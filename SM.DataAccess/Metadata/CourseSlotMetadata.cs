using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SM.Metadata
{    
	public class CourseSlotMetadata 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [Range(1, 240)]
        public byte MinutesDuration { get; set; }
        [Range(0,100)]
        public byte Order { get; set; }
        [Range(1, 28)]
        public byte Day { get; set; }
    }
}
