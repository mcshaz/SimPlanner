namespace SM.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Course
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public DateTime StartTime { get; set; }

        public Guid DepartmentId { get; set; }

        public Guid? OutreachingDepartmentId { get; set; }

        public byte FacultyNoRequired { get; set; }

        [StringLength(256)]
        public string ParticipantVideoFilename { get; set; }

        [StringLength(256)]
        public string FeedbackSummaryFilename { get; set; }

        public Guid CourseTypeId { get; set; }

        public virtual Department Department { get; set; }

        public virtual Department OutreachingDepartment { get; set; }

        public virtual CourseType CourseType { get; set; }

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

    }
}
