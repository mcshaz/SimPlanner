using System;

namespace SP.DTOs.Enums
{
    [Flags]
    public enum CourseParticipantEmailStatus
    {
        UpToDate = 0, ParticipantRolesChanged = 1, ManikinsChanged = 2, ActivitiesChanged = 4, CourseDatesChanged = 8, FacultyMeetingDatesChanged = 16
    }
}
