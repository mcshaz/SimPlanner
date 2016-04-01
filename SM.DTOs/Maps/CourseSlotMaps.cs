using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.Dto.Maps
{
    internal static class CourseSlotMaps
    {        internal static Func<CourseSlotDto, CourseSlot> mapToRepo()
        {
            return m => new CourseSlot
            {
                Id = m.Id,
                MinutesDuration = m.MinutesDuration,
                Day = m.Day, 
                ActivityId = m.ActivityId,
                CourseFormatId = m.CourseFormatId,
                Order = m.Order
            };
        }

        internal static Expression<Func<CourseSlot, CourseSlotDto>> mapFromRepo()
        {
            return m => new CourseSlotDto
            {
                Id = m.Id,
                MinutesDuration = m.MinutesDuration,
                Day = m.Day,
                ActivityId = m.ActivityId,
                CourseFormatId = m.CourseFormatId,
                Order = m.Order
            };
        }
    }
}
