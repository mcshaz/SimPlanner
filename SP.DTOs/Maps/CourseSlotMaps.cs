using SP.DataAccess;
using System;
using System.Linq.Expressions;
namespace SP.Dto.Maps
{
    internal static class CourseSlotMaps
    {        internal static Func<CourseSlotDto, CourseSlot> mapToRepo()
        {
            return m => new CourseSlot
            {
                Id = m.Id,
                MinutesDuration = m.MinutesDuration,
                Day = m.Day, 
                IsActive = m.IsActive,
                ActivityId = m.ActivityId,
                CourseFormatId = m.CourseFormatId,
                Order = m.Order,
                SimultaneousStreams = m.SimultaneousStreams
            };
        }

        internal static Expression<Func<CourseSlot, CourseSlotDto>> mapFromRepo()
        {
            return m => new CourseSlotDto
            {
                Id = m.Id,
                MinutesDuration = m.MinutesDuration,
                Day = m.Day,
                IsActive = m.IsActive,
                ActivityId = m.ActivityId,
                CourseFormatId = m.CourseFormatId,
                Order = m.Order,
                SimultaneousStreams = m.SimultaneousStreams
            };
        }
    }
}
