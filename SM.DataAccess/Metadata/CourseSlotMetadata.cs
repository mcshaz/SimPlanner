using System;
using System.ComponentModel;
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

        internal const int MaxStreams = 16;

        [Range(1,MaxStreams)]
        [DefaultValue(1)]
        public byte SimultaneousStreams { get; set; }
    }
}
