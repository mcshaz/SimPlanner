using System;

namespace SP.DataAccess.Data.Interfaces
{
    public interface ICourseDay
    {
        DateTime StartUtc {get;}
        int DurationMins { get; set; }
        int Day { get; }
    }
}
