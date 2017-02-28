﻿using System;

namespace SP.DataAccess.Data.Interfaces
{
    public interface ICourseDay
    {
        DateTime StartFacultyUtc {get;}
        int DurationFacultyMins { get; set; }
        DateTime StartParticipantUtc { get; set; }
        int DurationParticipantMins { get; set; }
        int Day { get; }
    }
}
