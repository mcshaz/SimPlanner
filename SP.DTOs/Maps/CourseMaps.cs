using SP.DataAccess;
using System;
using System.Linq;
namespace SP.Dto.Maps
{
    internal class CourseMaps: DomainDtoMap<Course, CourseDto>
    {
        public CourseMaps() : base(m => new Course
            {
                Id = m.Id,
                StartUtc = m.StartUtc,
                DurationMins = m.DurationMins,
                DepartmentId = m.DepartmentId,
                EmailSequence = m.EmailSequence,
                OutreachingDepartmentId = m.OutreachingDepartmentId,
                FacultyNoRequired = m.FacultyNoRequired,
                ParticipantVideoFilename = m.ParticipantVideoFilename,
                FeedbackSummaryFilename = m.FeedbackSummaryFilename,
                CourseFormatId = m.CourseFormatId,
                Cancelled = m.Cancelled,
                RoomId = m.RoomId,
                FacultyMeetingRoomId = m.FacultyMeetingRoomId,
                FacultyMeetingDuration = m.FacultyMeetingDuration,
                FacultyMeetingUtc = m.FacultyMeeting
        },
            m => new CourseDto
            {
                Id = m.Id,
                StartUtc = m.StartUtc,
                DurationMins = m.DurationMins,
                DepartmentId = m.DepartmentId,
                EmailSequence = m.EmailSequence,
                OutreachingDepartmentId = m.OutreachingDepartmentId,
                FacultyNoRequired = m.FacultyNoRequired,
                ParticipantVideoFilename = m.ParticipantVideoFilename,
                FeedbackSummaryFilename = m.FeedbackSummaryFilename,
                CourseFormatId = m.CourseFormatId,
                RoomId = m.RoomId,
                FacultyMeetingRoomId = m.FacultyMeetingRoomId,
                FacultyMeetingDuration = m.FacultyMeetingDuration,
                FacultyMeeting = m.FacultyMeetingUtc,
                Cancelled = m.Cancelled
            })
        { }

        static void IsAllowed(string[] includes,params string[] allowed)
        {
            var disallowed = includes.Except(includes);
            if (disallowed.Any())
            {
                throw new ArgumentException(
                    string.Format("the include parameter(s){0} are not allowed: allowed parameters include ({1})",
                    string.Join(",", disallowed), string.Join(",", allowed)));
            }
        }

    }
}
