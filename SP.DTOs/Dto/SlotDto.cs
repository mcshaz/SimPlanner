using System;
using System.Collections.Generic;
namespace SP.Dto
{
    public abstract class SlotDto
    {
        public Guid Id { get; set; }
        public byte MinutesDuration { get; set; }
        public byte Day { get; set; }
        public int Order { get; set; }
        public virtual ICollection<CourseFormatDto> CourseTypes { get; set; }
    }
}
