using SM.DataAccess;
using SM.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
namespace SM.Dto
{
    [MetadataType(typeof(SlotMetadata))]
    public class ScenarioSlotDto : SlotDto
    {

        internal static Func<ScenarioSlotDto, ScenarioSlot>  mapToRepo = m => new ScenarioSlot {
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
