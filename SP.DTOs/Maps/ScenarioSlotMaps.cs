using SP.DataAccess;

namespace SP.Dto.Maps
{
    internal class ScenarioSlotMaps: DomainDtoMap<CourseSlot, CourseSlotDto>
    {
        public ScenarioSlotMaps() : base(m => new CourseSlot
            {
                Id = m.Id,
                MinutesDuration = m.MinutesDuration,
                Day = m.Day,
                Order = m.Order
                //CourseTypes = m.CourseTypes
            },
            m => new CourseSlotDto
            {
                Id = m.Id,
                MinutesDuration = m.MinutesDuration,
                Day = m.Day,
                Order = m.Order
                //CourseTypes = m.CourseTypes
            })
        { }
    }
}
