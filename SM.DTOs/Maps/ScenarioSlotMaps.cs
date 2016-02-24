using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.DTOs.Maps
{
    internal static class ScenarioSlotMaps
    {        internal static Func<ScenarioSlotDto, ScenarioSlot>  mapToRepo = m => new ScenarioSlot {
            Id = m.Id,
            MinutesDuration = m.MinutesDuration,
            Day = m.Day,
            Order = m.Order
            //CourseTypes = m.CourseTypes
        };

        internal static Expression<Func<ScenarioSlot, ScenarioSlotDto>> mapFromRepo= m => new ScenarioSlotDto
        {
            Id = m.Id,
            MinutesDuration = m.MinutesDuration,
            Day = m.Day,
            Order = m.Order
            //CourseTypes = m.CourseTypes
        };
    }
}
