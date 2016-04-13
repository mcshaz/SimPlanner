using SM.DataAccess.Enums;
using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SM.Dto
{
    [MetadataType(typeof(CourseFormatMetadata))]
    public class CourseFormatDto
	{
        public Guid Id { get; set; }
        public string Description { get; set; }
        public byte DaysDuration { get; set; }
        public Guid CourseTypeId { get; set; }

        public virtual CourseTypeDto CourseType { get; set; }
        public ICollection<CourseDto> Courses { get; set; }
        public ICollection<CourseSlotDto> CourseSlots { get; set; }
    }
}
