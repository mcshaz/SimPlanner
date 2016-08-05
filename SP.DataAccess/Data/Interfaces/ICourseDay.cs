using System;

namespace SP.DataAccess.Data.Interfaces
{
    public interface ICourseDay
    {
        DateTime StartUtc {get;}
        TimeSpan Duration { get; set; }
        int Day { get; }
    }
}
