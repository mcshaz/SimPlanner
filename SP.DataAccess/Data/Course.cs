namespace SP.DataAccess
{
    using Data.Interfaces;
    using SP.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Utilities;

    [MetadataType(typeof(CourseMetadata))]
    public class Course : ICourseDay
    {
        public Guid Id { get; set; }

        /// <summary>
        /// To implement ICourseDay - refers to duration in minutes for day 1.
        /// </summary>
        public int DurationMins { get; set; }
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

        private DateTime _startUtc;
        public DateTime StartUtc
        {
            get { return _startUtc; }
            set { _startUtc = value.AsUtc(); _startLocal = default(DateTime); }
        }
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
        DateTime _startLocal;
        [NotMapped]
        public DateTime StartLocal
        {
            get
            {
                return _startLocal == default(DateTime)
                    ? (_startLocal = TimeZoneInfo.ConvertTimeFromUtc(StartUtc, Department.Institution.TimeZone))
                    : _startLocal;
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

        [NotMapped]
        //ICourseDay implementation
        int ICourseDay.Day
        {
            get { return 1; }
        }

    }

}
