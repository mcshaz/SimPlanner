using SP.DataAccess;
using SP.Dto;
using System;
using System.Linq;
using System.Linq.Expressions;
namespace SP.Dto.Maps
{
    internal static class CourseMaps
    {
        internal static Func<CourseDto, Course> MapToDomain()
        {
            return m => new Course
            {
                Id = m.Id,
                StartUtc = m.Start,
                Duration = m.Duration,
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
                FacultyMeetingUtc = m.FacultyMeeting,
                CreatedUtc = m.Created,
                LastModifiedUtc = m.LastModified
            };
        }
        /*
        internal static Expression<Func<Course, CourseDto>> MapFromDomain(string[] includes) {
            if (["Scenarios", "CourseType", "CourseParticipants", "ScenarioFacultyRoles"])
            {

            } */
        internal static Expression<Func<Course, CourseDto>> MapFromDomain()
        {
            return m => new CourseDto
            {
                Id = m.Id,
                Start = m.StartUtc,
                Duration = m.Duration,
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
                Created = m.CreatedUtc,
                LastModified = m.LastModifiedUtc
            };
            //Department = m.Department,

            //OutreachingDepartment = m.OutreachingDepartment,

            //CourseType = m.CourseType,

            //CourseParticipants = m.CourseParticipants,

            //ScenarioFacultyRoles = m.ScenarioFacultyRoles
        }

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

        /*
        internal static Expression<Func<Course, CourseDto>> mapBriefFromRepo = m => new CourseDto
        {
            Id = m.Id,
            StartTime = m.StartTime,
            DepartmentId = m.DepartmentId,
            OutreachingDepartmentId = m.OutreachingDepartmentId,
            FacultyNoRequired = m.FacultyNoRequired,
            CourseTypeId = m.CourseTypeId,

            CourseParticipants = m.CourseParticipants.Select(cp=>new CourseParticipantDto
            {
                ParticipantId = cp.ParticipantId,
                CourseId = cp.CourseId,
                IsConfirmed = cp.IsConfirmed,
                IsFaculty = cp.IsFaculty,
                DepartmentId = cp.DepartmentId,
                ProfessionalRoleId = cp.ProfessionalRoleId
            }).ToList()

            //Department = m.Department,

            //OutreachingDepartment = m.OutreachingDepartment,

            //CourseType = m.CourseType,


            //ScenarioFacultyRoles = m.ScenarioFacultyRoles
        };
        */

    }
}
