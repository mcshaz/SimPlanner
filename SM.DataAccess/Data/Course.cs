namespace SM.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Course
    {
        [Key]
        public int Id { get; set; }

        public DateTime StartTime { get; set; }

        public int DepartmentId { get; set; }

        public int? OutreachingDepartmentId { get; set; }

        public byte FacultyNoRequired { get; set; }

        [StringLength(256)]
        public string ParticipantVideoFilename { get; set; }

        [StringLength(256)]
        public string FeedbackSummaryFilename { get; set; }

        public int CourseTypeId { get; set; }

        public virtual Department Department { get; set; }

        public virtual Department OutreachingDepartment { get; set; }

        public virtual CourseType CourseType { get; set; }

		ICollection<CourseParticipant> _participants; 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseParticipant> Participants
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

    }
}
