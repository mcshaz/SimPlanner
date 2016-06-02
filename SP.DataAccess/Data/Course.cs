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

        public DateTime StartTime { get; set; }

        public DateTime FinishTime { get; set; }

        public DateTime? FacultyMeetingTime { get; set; }

        public Guid DepartmentId { get; set; }

        public Guid? OutreachingDepartmentId { get; set; }

        public Guid RoomId { get; set; }

        public Guid? FacultyMeetingRoomId { get; set; }

        public int? FacultyMeetingDuration { get; set; }

        public byte EmailSequence { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastModified { get; set; }

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

    }

    public static class CourseExtensions
    {
        public static void CalculateFinishTime(this Course course)
        {

            //todo account for multiday courses
            course.FinishTime = course.StartTime + TimeSpan.FromMinutes(course.CourseFormat.CourseSlots.Sum(cs => cs.MinutesDuration));
        }

        public static DateTime LocalStart(this Course course)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(course.StartTime, TimeZoneInfo.FindSystemTimeZoneById(course.Department.Institution.StandardTimeZone));
        }

        public static DateTime LocalFinish(this Course course)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(course.FinishTime, TimeZoneInfo.FindSystemTimeZoneById(course.Department.Institution.StandardTimeZone));
        }
    }
}
