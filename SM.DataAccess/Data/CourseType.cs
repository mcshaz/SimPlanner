namespace SM.DataAccess
{
    using Enums;
    using SM.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(CourseTypeMetadata))]
    public partial class CourseType
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public bool IsInstructorCourse { get; set; }

        public byte DaysDuration { get; set; }

        public Emersion? EmersionCategory { get; set; }

        public string Abbrev { get; set; }

        ICollection<Department> _departments;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Department> Departments
        {
            get
            {
                return _departments ?? (_departments = new List<Department>());
            }
            set
            {
                _departments = value;
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

		ICollection<Course> _courses; 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Course> Courses
		{
			get
			{
				return _courses ?? (_courses = new List<Course>());
			}
			set
			{
				_courses = value;
			}
		}

		ICollection<CourseSlot> _courseEvents; 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseSlot> CourseEvents
		{
			get
			{
				return _courseEvents ?? (_courseEvents = new List<CourseSlot>());
			}
			set
			{
				_courseEvents = value;
			}
		}

		ICollection<ScenarioSlot> _scenarioEvents; 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScenarioSlot> ScenarioEvents
		{
			get
			{
				return _scenarioEvents ?? (_scenarioEvents = new List<ScenarioSlot>());
			}
			set
			{
				_scenarioEvents = value;
			}
		}

		ICollection<ScenarioRoleDescription> _scenarioRoles; 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScenarioRoleDescription> ScenarioRoles
		{
			get
			{
				return _scenarioRoles ?? (_scenarioRoles = new List<ScenarioRoleDescription>());
			}
			set
			{
				_scenarioRoles = value;
			}
		}
    }
}
