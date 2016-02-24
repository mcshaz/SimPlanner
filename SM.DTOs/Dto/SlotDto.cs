using System;
using System.Collections.Generic;
namespace SM.Dto
{
    public abstract class SlotDto
    {
        public Guid Id { get; set; }
        public int MinutesDuration { get; set; }
        public byte Day { get; set; }
        public int Order { get; set; }
        public ICollection<CourseTypeDto> CourseTypes { get; set; }
    }
}
