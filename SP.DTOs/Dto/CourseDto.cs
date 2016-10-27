using Newtonsoft.Json;
using SP.DataAccess;
using SP.DataAccess.Data.Interfaces;
using SP.Dto.Utilities;
using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SP.Dto
{
    [MetadataType(typeof(CourseMetadata))]
    public class CourseDto : ICourseDay
    {
        public Guid Id { get; set; }
        public int DurationMins { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid? OutreachingDepartmentId { get; set; }
        public Guid RoomId { get; set; }
        public Guid? FacultyMeetingRoomId { get; set; }
        public int? FacultyMeetingDuration { get; set; }
        public byte FacultyNoRequired { get; set; }
        public byte EmailSequence { get; set; }
        public Guid CourseFormatId { get; set; }
        public string ParticipantVideoFilename { get; set; }
        public string FeedbackSummaryFilename { get; set; }
        public bool Cancelled { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime? FacultyMeeting { get; set; }

        public DepartmentDto Department { get; set; }
        public DepartmentDto OutreachingDepartment { get; set; }
        public CourseFormatDto CourseFormat { get; set; }
        public RoomDto Room { get; set; }
        public RoomDto FacultyMeetingRoom { get; set; }

        public virtual ICollection<CourseParticipantDto> CourseParticipants { get; set; }
        public virtual ICollection<CourseScenarioFacultyRoleDto> CourseScenarioFacultyRoles { get; set; }
        public virtual ICollection<CourseSlotActivityDto> CourseSlotActivities { get; set; }
        public virtual ICollection<CourseSlotManikinDto> CourseSlotManikins { get; set; }
        public virtual ICollection<CourseSlotPresenterDto> CourseSlotPresenters { get; set; }
        public virtual ICollection<CourseDayDto> CourseDays { get; set; }

        //ICourseDay implementation
        int ICourseDay.Day
        {
            get { return 1; }
        }
    }
    public static class CourseExtensions
    {
        public static IEnumerable<ICourseDay> AllDays(this CourseDto course)
        {
            return (new[] { (ICourseDay)course }).Concat(course.CourseDays).OrderBy(cd => cd.Day);
        }

        public static IEnumerable<ICourseDay> AllDays(this Course course)
        {
            return (new[] { (ICourseDay)course }).Concat(course.CourseDays).OrderBy(cd => cd.Day);
        }

        public static ICourseDay LastDay(this Course course)
        {
            var days = course.CourseFormat.DaysDuration;
            return days > 1
                ? course.CourseDays.First(cd => cd.Day == days)
                : (ICourseDay)course;
        }

        public static DateTime FinishCourseUtc(this Course course)
        {
            var lastDay = course.LastDay();
            return lastDay.StartUtc + TimeSpan.FromMinutes(lastDay.DurationMins);
        }

        public static DateTime FinishCourseLocal(this Course course)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(course.FinishCourseUtc(), course.Department.Institution.TimeZone);
        }

        public static DateTime FinishCourseDayUtc(this ICourseDay courseDay)
        {
            return courseDay.StartUtc + TimeSpan.FromMinutes(courseDay.DurationMins);
        }

        public static DateTime FinishCourseDayLocal(this ICourseDay courseDay)
        {
            var dpt = courseDay.Day == 1
                ? ((Course)courseDay).Department
                : ((CourseDay)courseDay).Course.Department;
            return TimeZoneInfo.ConvertTimeFromUtc(courseDay.FinishCourseDayUtc(), dpt.Institution.TimeZone);
        }
    }
}
