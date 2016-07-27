namespace SP.DataAccess
{
    using SP.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    [MetadataType(typeof(CourseMetadata))]
    public class Course
    {
        public Guid Id { get; set; }

        private DateTime _startTimeUtc;
        public DateTime StartTimeUtc
        {
            get
            {
                return _startTimeUtc;
            }
            set
            {
                _startTimeUtc = AsUtc(value);
            }
        }

        private DateTime _finishTimeUtc;
        public DateTime FinishTimeUtc
        {
            get {return _finishTimeUtc;} set{ _finishTimeUtc = AsUtc(value); }
        }

        private DateTime? _facultyMeetingTimeUtc;
        public DateTime? FacultyMeetingTimeUtc
        {
            get
            {
                return _facultyMeetingTimeUtc;
            }
            set
            {
                _facultyMeetingTimeUtc = value.HasValue?AsUtc(value.Value):(DateTime?)null;
            }
        }

        public Guid DepartmentId { get; set; }

        public Guid? OutreachingDepartmentId { get; set; }

        public Guid RoomId { get; set; }

        public Guid? FacultyMeetingRoomId { get; set; }

        public int? FacultyMeetingDuration { get; set; }

        public byte EmailSequence { get; set; }

        private DateTime _createdUtc;
        public DateTime CreatedUtc
        {
            get {return _createdUtc;}
            set { _createdUtc = AsUtc(value); }
        }

        private DateTime _lastModifiedUtc;
        public DateTime LastModifiedUtc
        {
            get {return _lastModifiedUtc;}
            set { _lastModifiedUtc = AsUtc(value); }
        }

        public byte FacultyNoRequired { get; set; }

        public string ParticipantVideoFilename { get; set; }

        public string FeedbackSummaryFilename { get; set; }

        public Guid CourseFormatId { get; set; }

        public virtual Department Department { get; set; }

        public virtual Department OutreachingDepartment { get; set; }

        public virtual CourseFormat CourseFormat { get; set; }

        public virtual Room Room { get; set; }

        public virtual Room FacultyMeetingRoom { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseParticipant> CourseParticipants { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseScenarioFacultyRole> CourseScenarioFacultyRoles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseSlotManequin> CourseSlotManequins { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseSlotScenario> CourseSlotScenarios { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseSlotPresenter> CourseSlotPresenters { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChosenTeachingResource> ChosenTeachingResources { get; set; }

        private static DateTime AsUtc(DateTime inptDate)
        {
            switch (inptDate.Kind)
            {
                case DateTimeKind.Local:
                    throw new ArgumentException("DateTime MUST NOT be of kind local");
                case DateTimeKind.Utc:
                    return inptDate;
                default:
                    return DateTime.SpecifyKind(inptDate, DateTimeKind.Utc);
            }
        }

    }

    public static class CourseExtensions
    {
        public static void CalculateFinishTime(this Course course)
        {

            //todo account for multiday courses
            course.FinishTimeUtc = course.StartTimeUtc + TimeSpan.FromMinutes(course.CourseFormat.CourseSlots.Sum(cs => cs.MinutesDuration));
        }

        public static DateTime LocalStart(this Course course)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(course.StartTimeUtc, TimeZoneInfo.FindSystemTimeZoneById(course.Department.Institution.StandardTimeZone));
        }

        public static DateTime LocalFinish(this Course course)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(course.FinishTimeUtc, TimeZoneInfo.FindSystemTimeZoneById(course.Department.Institution.StandardTimeZone));
        }
    }
}
