using SP.DataAccess;
namespace SP.Dto.Maps
{
    internal class CourseMaps: DomainDtoMap<Course, CourseDto>
    {
        public CourseMaps() : base(m => new Course
            {
                Id = m.Id,
                StartFacultyUtc = m.StartFacultyUtc,
                DurationFacultyMins = m.DurationFacultyMins,
                DepartmentId = m.DepartmentId,
                Version = m.Version,
                OutreachingDepartmentId = m.OutreachingDepartmentId,
                FacultyNoRequired = m.FacultyNoRequired,
                ParticipantVideoFilename = m.ParticipantVideoFilename,
                FeedbackSummaryFilename = m.FeedbackSummaryFilename,
                CourseFormatId = m.CourseFormatId,
                Cancelled = m.Cancelled,
                RoomId = m.RoomId,
                FacultyMeetingRoomId = m.FacultyMeetingRoomId,
                FacultyMeetingDuration = m.FacultyMeetingDuration,
                FacultyMeetingUtc = m.FacultyMeeting,
                DurationParticipantMins = m.DurationParticipantMins,
                StartParticipantUtc = m.StartParticipantUtc
        },
            m => new CourseDto
            {
                Id = m.Id,
                StartFacultyUtc = m.StartFacultyUtc,
                DurationFacultyMins = m.DurationFacultyMins,
                DepartmentId = m.DepartmentId,
                Version = m.Version,
                OutreachingDepartmentId = m.OutreachingDepartmentId,
                FacultyNoRequired = m.FacultyNoRequired,
                ParticipantVideoFilename = m.ParticipantVideoFilename,
                FeedbackSummaryFilename = m.FeedbackSummaryFilename,
                CourseFormatId = m.CourseFormatId,
                RoomId = m.RoomId,
                FacultyMeetingRoomId = m.FacultyMeetingRoomId,
                FacultyMeetingDuration = m.FacultyMeetingDuration,
                FacultyMeeting = m.FacultyMeetingUtc,
                Cancelled = m.Cancelled,
                DurationParticipantMins = m.DurationParticipantMins,
                StartParticipantUtc = m.StartParticipantUtc
            })
        { }
    }
}
