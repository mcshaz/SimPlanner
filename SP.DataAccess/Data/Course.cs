namespace SP.DataAccess
{
    using Data.Interfaces;
    using SP.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Utilities;

    [MetadataType(typeof(CourseMetadata))]
    public class Course : ICourseDay, ICreated
    {
        public Guid Id { get; set; }

        /// <summary>
        /// To implement ICourseDay - refers to duration in minutes for day 1.
        /// </summary>
        public int DurationFacultyMins { get; set; }
        public int DurationParticipantMins { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid? OutreachingDepartmentId { get; set; }
        public Guid RoomId { get; set; }
        public Guid? FacultyMeetingRoomId { get; set; }
        public int? FacultyMeetingDuration { get; set; }
        public int Version { get; set; }
        public byte FacultyNoRequired { get; set; }
        public string ParticipantVideoFilename { get; set; }
        public string FeedbackSummaryFilename { get; set; }
        public bool Cancelled { get; set; }

        public Guid CourseFormatId { get; set; }
        private DateTime _createdUtc;
        public DateTime CreatedUtc { get { return _createdUtc; } set { _createdUtc = value.AsUtc(); } }
        private DateTime _courseDatesLastModified;
        public DateTime CourseDatesLastModified { get { return _courseDatesLastModified; } set { _courseDatesLastModified = value.AsUtc(); } }
        private DateTime _facultyMeetingDatesLastModified;
        public DateTime FacultyMeetingDatesLastModified { get { return _facultyMeetingDatesLastModified; } set { _facultyMeetingDatesLastModified = value.AsUtc(); } }

        private DateTime _startFacultyUtc;
        public DateTime StartFacultyUtc
        {
            get { return _startFacultyUtc; }
            set { _startFacultyUtc = value.AsUtc(); _startFacultyLocal = default(DateTime); }
        }

        public int DelayStartParticipantMins { get; set; }

        private DateTime? _facultyMeetingUtc;
        public DateTime? FacultyMeetingUtc
        {
            get
            {
                return _facultyMeetingUtc;
            }
            set
            {
                _facultyMeetingUtc = value.HasValue ? value.Value.AsUtc() : (DateTime?)null;
                _facultyMeetingLocal = default(DateTime);
            }
        }
        DateTime _startFacultyLocal;
        [NotMapped]
        public DateTime StartFacultyLocal
        {
            get
            {
                return _startFacultyLocal == default(DateTime)
                    ? (_startFacultyLocal = TimeZoneInfo.ConvertTimeFromUtc(StartFacultyUtc, Department.Institution.TimeZone))
                    : _startFacultyLocal;
            }
        }

        DateTime _facultyMeetingLocal;
        [NotMapped]
        public DateTime? FacultyMeetingLocal
        {
            get
            {
                return FacultyMeetingUtc.HasValue
                    ? _facultyMeetingLocal == default(DateTime)
                        ? TimeZoneInfo.ConvertTimeFromUtc(FacultyMeetingUtc.Value, Department.Institution.TimeZone)
                        : _facultyMeetingLocal
                    : (DateTime?)null;
            }
        }

        public virtual Department Department { get; set; }
        public virtual Department OutreachingDepartment { get; set; }
        public virtual CourseFormat CourseFormat { get; set; }
        public virtual Room Room { get; set; }
        public virtual Room FacultyMeetingRoom { get; set; }

        public virtual ICollection<CourseParticipant> CourseParticipants { get; set; }
        public virtual ICollection<CourseScenarioFacultyRole> CourseScenarioFacultyRoles { get; set; }
        public virtual ICollection<CourseSlotManikin> CourseSlotManikins { get; set; }
        public virtual ICollection<CourseSlotActivity> CourseSlotActivities { get; set; }
        public virtual ICollection<CourseSlotPresenter> CourseSlotPresenters { get; set; }
        public virtual ICollection<CourseDay> CourseDays { get; set; }
        public virtual ICollection<CourseFacultyInvite> FacultyInvites { get; set; } 
        public virtual ICollection<CourseHangfireJob> HangfireJobs { get; set; }

        [NotMapped]
        //ICourseDay implementation
        int ICourseDay.Day
        {
            get { return 1; }
        }

    }

    public static class CourseExtensions
    {

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

        #region FacultyTimes
        public static DateTime FinishCourseFacultyUtc(this Course course)
        {
            var lastDay = course.LastDay();
            return lastDay.StartFacultyUtc + TimeSpan.FromMinutes(lastDay.DurationFacultyMins);
        }

        public static DateTime FinishCourseFacultyLocal(this Course course)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(course.FinishCourseFacultyUtc(), course.Department.Institution.TimeZone);
        }

        public static DateTime FinishCourseDayFacultyUtc(this ICourseDay courseDay)
        {
            return courseDay.StartFacultyUtc + TimeSpan.FromMinutes(courseDay.DurationFacultyMins);
        }

        public static DateTime FinishCourseDayFacultyLocal(this ICourseDay courseDay)
        {
            var dpt = courseDay.Day == 1
                ? ((Course)courseDay).Department
                : ((CourseDay)courseDay).Course.Department;
            return TimeZoneInfo.ConvertTimeFromUtc(courseDay.FinishCourseDayFacultyUtc(), dpt.Institution.TimeZone);
        }

        #endregion //FacultyTimes
        #region ParticipantTimes

        public static DateTime StartParticipantUtc(this ICourseDay courseDay)
        {
            return courseDay.StartFacultyUtc + TimeSpan.FromMinutes(courseDay.DelayStartParticipantMins);
        }

        public static DateTime StartParticipantLocal(this Course course)
        {
            return course.StartFacultyLocal + TimeSpan.FromMinutes(course.DelayStartParticipantMins);
        }

        public static DateTime FinishCourseParticipantUtc(this Course course)
        {
            var lastDay = course.LastDay();
            return lastDay.StartParticipantUtc() + TimeSpan.FromMinutes(lastDay.DurationParticipantMins);
        }

        public static DateTime FinishCourseParticipantLocal(this Course course)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(course.FinishCourseParticipantUtc(), course.Department.Institution.TimeZone);
        }

        public static DateTime FinishCourseDayParticipantUtc(this ICourseDay courseDay)
        {
            return courseDay.StartParticipantUtc() + TimeSpan.FromMinutes(courseDay.DurationParticipantMins);
        }

        public static DateTime FinishCourseDayParticipantLocal(this ICourseDay courseDay)
        {
            var dpt = courseDay.Day == 1
                ? ((Course)courseDay).Department
                : ((CourseDay)courseDay).Course.Department;
            return TimeZoneInfo.ConvertTimeFromUtc(courseDay.FinishCourseDayParticipantUtc(), dpt.Institution.TimeZone);
        }
        #endregion // ParticipantTimes
    }

}
