using SP.DataAccess;namespace SP.Dto.Maps
{
    internal class CourseSlotMaps: DomainDtoMap<CourseSlot, CourseSlotDto> 
    {
        public CourseSlotMaps() : base(m => new CourseSlot
            {
                Id = m.Id,
                MinutesDuration = m.MinutesDuration,
                Day = m.Day, 
                IsActive = m.IsActive,
                ActivityId = m.ActivityId,
                CourseFormatId = m.CourseFormatId,
                Order = m.Order,
                SimultaneousStreams = m.SimultaneousStreams
            },
            m => new CourseSlotDto
            {
                Id = m.Id,
                MinutesDuration = m.MinutesDuration,
                Day = m.Day,
                IsActive = m.IsActive,
                ActivityId = m.ActivityId,
                CourseFormatId = m.CourseFormatId,
                Order = m.Order,
                SimultaneousStreams = m.SimultaneousStreams
            })
        { }
    }
}
