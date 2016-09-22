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
    public class Course : ICourseDay, ITimeTracking
    {
        public Guid Id { get; set; }

        private DateTime _startUtc;
        public DateTime StartUtc
        {
            get { return _startUtc; }
            set { _startUtc = value.AsUtc(); }
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
            }
        }

        public Guid DepartmentId { get; set; }

        public Guid? OutreachingDepartmentId { get; set; }

        public Guid RoomId { get; set; }

        public Guid? FacultyMeetingRoomId { get; set; }

        public int? FacultyMeetingDuration { get; set; }

        public byte EmailSequence { get; set; }

        public int Version { get; internal set; }

        private DateTime _createdUtc;
        public DateTime CreatedUtc
        {
            get {
                return _createdUtc;
            }
            set {
                _createdUtc = value.AsUtc();
            }
        }

        private DateTime _lastModifiedUtc;
        public DateTime LastModifiedUtc
        {
            get {
                return _lastModifiedUtc;
            }
            set {
                _lastModifiedUtc = value.AsUtc();
            }
        }

        public byte FacultyNoRequired { get; set; }

        public string ParticipantVideoFilename { get; set; }

        public string FeedbackSummaryFilename { get; set; }

        public bool Cancelled { get; set; }

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
        public virtual ICollection<CourseSlotManikin> CourseSlotManikins { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseSlotScenario> CourseSlotScenarios { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseSlotPresenter> CourseSlotPresenters { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChosenTeachingResource> ChosenTeachingResources { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseDay> CourseDays { get; set; }

        //ICourseDay implementation
        [NotMapped]
        public int Day
        {
            get { return 1; }
        }
    }

    public static class CourseExtensions
    {

        public static ICourseDay LastDay(this Course course)
        {
            var days = course.CourseFormat.DaysDuration;
            return days > 1
                ?course.CourseDays.First(cd => cd.Day == days)
                :(ICourseDay)course;
        }

        public static DateTime FinishTimeUtc(this Course course)
        {
            var lastDay = course.LastDay();
            return lastDay.StartUtc + TimeSpan.FromMinutes(lastDay.DurationMins);
        }
    }

}
