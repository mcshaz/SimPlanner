using System;

namespace SP.DataAccess.Data.Interfaces
{
    public interface ICourseDay
    {
        DateTime StartFacultyUtc {get;}
        int DurationFacultyMins { get; set; }
        int DelayStartParticipantMins { get; set; }
        int DurationParticipantMins { get; set; }
        int Day { get; }
    }
}
