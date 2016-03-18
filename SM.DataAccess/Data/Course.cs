namespace SM.DataAccess
{
    using SM.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(CourseMetadata))]
    public class Course
    {
        public Guid Id { get; set; }

        public DateTime StartTime { get; set; }

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

        public virtual CourseType CourseType { get; set; }

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

        ICollection<ScenarioFacultyRole> _scenarioFacultyRoles;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScenarioFacultyRole> ScenarioFacultyRoles
        {
            get
            {
                return _scenarioFacultyRoles ?? (_scenarioFacultyRoles = new List<ScenarioFacultyRole>());
            }
            set
            {
                _scenarioFacultyRoles = value;
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

    }
}
