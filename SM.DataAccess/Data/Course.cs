namespace SM.DataAccess
{
    using SM.Metadata;
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

        public byte FacultyNoRequired { get; set; }

        public string ParticipantVideoFilename { get; set; }

        public string FeedbackSummaryFilename { get; set; }

        public Guid CourseTypeId { get; set; }

        public virtual Department Department { get; set; }

        public virtual Department OutreachingDepartment { get; set; }

        public virtual CourseFormat CourseFormat { get; set; }

        public virtual Room Room { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseParticipant> CourseParticipants { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Scenario> Scenarios { get; set; } //todo IS THIS NEEDED??????

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseScenarioFacultyRole> CourseScenarioFacultyRoles { get; set; }

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
    }
}
