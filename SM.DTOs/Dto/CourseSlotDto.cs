using SM.DataAccess;
using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
namespace SM.Dto
{
    [MetadataType(typeof(CourseSlotMetadata))]
    public class CourseSlotDto : Slot
    {
        public string Name { get; set; }
        public byte MinimumFaculty { get; set; }
        public byte MaximumFaculty { get; set; }
        public ICollection<CourseTeachingResourceDto> DefaultResources { get; set; }

        internal static Func<CourseSlotDto, CourseSlot> mapToRepo = m => new CourseSlot
        {
            Id = m.Id,
            MinutesDuration = m.MinutesDuration,
            Day = m.Day,
            Order = m.Order,

            Name = m.Name,
            MinimumFaculty = m.MinimumFaculty,
            MaximumFaculty = m.MaximumFaculty
        };

        internal static Expression<Func<CourseSlot, CourseSlotDto>> mapFromRepo = m => new CourseSlotDto
        {
            Id = m.Id,
            MinutesDuration = m.MinutesDuration,
            Day = m.Day,
            Order = m.Order,

            Name = m.Name,
            MinimumFaculty = m.MinimumFaculty,
            MaximumFaculty = m.MaximumFaculty,
            //DefaultResources = m.DefaultResources
        };
    }
}
