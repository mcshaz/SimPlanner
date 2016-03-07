using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.DTOs.Maps
{
    internal static class CourseSlotMaps
    {        internal static Func<CourseSlotDto, CourseSlot> mapToRepo()
        {
            return m => new CourseSlot
            {
                Id = m.Id,
                MinutesDuration = m.MinutesDuration,
                Day = m.Day,
                Order = m.Order,

                Name = m.Name,
                MinimumFaculty = m.MinimumFaculty,
                MaximumFaculty = m.MaximumFaculty
            };
        }

        internal static Expression<Func<CourseSlot, CourseSlotDto>> mapFromRepo()
        {
            return m => new CourseSlotDto
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
}
