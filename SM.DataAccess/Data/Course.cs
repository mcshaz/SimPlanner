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

		ICollection<CourseParticipant> _participants; 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseParticipant> CourseParticipants
		{
			get
			{
				return _participants ?? (_participants = new List<CourseParticipant>());
			}
			set
			{
				_participants = value;
			}
		}

		ICollection<Scenario> _scenarios; 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Scenario> Scenarios
		{
			get
			{
				return _scenarios ?? (_scenarios = new List<Scenario>());
			}
			set
			{
				_scenarios = value;
			}
		}

        ICollection<CourseScenarioFacultyRole> _courseScenarioFacultyRoles;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseScenarioFacultyRole> CourseScenarioFacultyRoles
        {
            get
            {
                return _courseScenarioFacultyRoles ?? (_courseScenarioFacultyRoles = new List<CourseScenarioFacultyRole>());
            }
            set
            {
                _courseScenarioFacultyRoles = value;
            }
        }

        ICollection<CourseSlotScenario> _courseSlotScenarios;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseSlotScenario> CourseSlotScenarios
        {
            get
            {
                return _courseSlotScenarios ?? (_courseSlotScenarios = new List<CourseSlotScenario>());
            }
            set
            {
                _courseSlotScenarios = value;
            }
        }

        ICollection<CourseSlotPresenter> _courseSlotPresenters;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseSlotPresenter> CourseSlotPresenters
        {
            get
            {
                return _courseSlotPresenters ?? (_courseSlotPresenters = new List<CourseSlotPresenter>());
            }
            set
            {
                _courseSlotPresenters = value;
            }
        }

        ICollection<ChosenTeachingResource> _chosenTeachingResources;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChosenTeachingResource> ChosenTeachingResources
        {
            get
            {
                return _chosenTeachingResources ?? (_chosenTeachingResources = new List<ChosenTeachingResource>());
            }
            set
            {
                _chosenTeachingResources = value;
            }
        }
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
