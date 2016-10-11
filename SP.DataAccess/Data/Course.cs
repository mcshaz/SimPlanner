namespace SP.DataAccess
{
    using Data.Interfaces;
    using Helpers;
    using SP.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    [MetadataType(typeof(CourseMetadata))]
    public class Course : ICourseDay
    {
        public Guid Id { get; set; }

        private DateTime _startUtc;
        public DateTime StartUtc
        {
            get { return _startUtc; }
            set { _startUtc = value.AsUtc(); _startLocal = default(DateTime); }
        }

        /// <summary>
        /// To implement ICourseDay - refers to duration in minutes for day 1.
        /// </summary>
        public int DurationMins { get; set; }
        private DateTime? _facultyMeetingUtc;
        public DateTime? FacultyMeetingUtc
        {
            get
            {
                return _facultyMeetingUtc;
            }
            set
            {
                _facultyMeetingUtc = value.HasValue? value.Value.AsUtc(): (DateTime?)null;
                _facultyMeetingLocal = default(DateTime);
            }
        }
        public Guid DepartmentId { get; set; }
        public Guid? OutreachingDepartmentId { get; set; }
        public Guid RoomId { get; set; }
        public Guid? FacultyMeetingRoomId { get; set; }
        public int? FacultyMeetingDuration { get; set; }
        public byte EmailSequence { get; set; }
        public byte FacultyNoRequired { get; set; }
        public string ParticipantVideoFilename { get; set; }
        public string FeedbackSummaryFilename { get; set; }
        public bool Cancelled { get; set; }
        public Guid CourseFormatId { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime CourseDatesLastModified { get; set; }
        public DateTime FacultyMeetingDatesLastModified { get; set; }

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

        //ICourseDay implementation
        [NotMapped]
        public int Day
        {
            get { return 1; }
        }

        [NotMapped]
        DateTime _startLocal;
        public DateTime StartLocal
        {
            get
            {
                return _startLocal == default(DateTime) 
                    ? (_startLocal = TimeZoneInfo.ConvertTimeFromUtc(StartUtc, Department.Institution.TimeZone))
                    : _startLocal;
            }
        }

        [NotMapped]
        DateTime _facultyMeetingLocal;
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
    }

    public static class CourseExtensions
    {
        public static IEnumerable<ICourseDay> AllDays(this Course course)
        {
            return (new[] { (ICourseDay)course }).Concat(course.CourseDays).OrderBy(cd=>cd.Day);
        } 
        public static ICourseDay LastDay(this Course course)
        {
            var days = course.CourseFormat.DaysDuration;
            return days > 1
                ?course.CourseDays.First(cd => cd.Day == days)
                :(ICourseDay)course;
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
            var dpt = courseDay.Day==1
                ? ((Course)courseDay).Department
                : ((CourseDay)courseDay).Course.Department;
            return TimeZoneInfo.ConvertTimeFromUtc(courseDay.FinishCourseDayUtc(), dpt.Institution.TimeZone);
        }
    }

}
