using SP.DataAccess;
using System;
using System.Linq;
namespace SP.Dto.Maps
{
    internal class CourseDayMaps: DomainDtoMap<CourseDay, CourseDayDto>
    {
        public CourseDayMaps() : base(
        m => new CourseDay
        {
            CourseId = m.CourseId,
            Day = m.Day,
            DurationFacultyMins =m.DurationFacultyMins,
            StartFacultyUtc = m.StartFacultyUtc,
            DurationParticipantMins = m.DurationParticipantMins,
            DelayStartParticipantMins = m.DelayStartParticipantMins
        },
        m => new CourseDayDto
        {
            CourseId = m.CourseId,
            Day = m.Day,
            DurationFacultyMins = m.DurationFacultyMins,
            StartFacultyUtc = m.StartFacultyUtc,
            DurationParticipantMins = m.DurationParticipantMins,
            DelayStartParticipantMins = m.DelayStartParticipantMins
        })
        { }

    }
}
