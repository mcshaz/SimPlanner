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
        public string Abbrev { get; set; }
        public bool IsInstructorCourse { get; set; }

        public Emersion? EmersionCategory { get; set; }

        ICollection<CourseActivity> _courseActivities;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseActivity> CourseActivities
        {
            get
            {
                return _courseActivities ?? (_courseActivities = new List<CourseActivity>());
            }
            set
            {
                _courseActivities = value;
            }
        }

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

        ICollection<FacultySimRole> _facultySimRoles;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FacultySimRole> FacultySimRoles
        {
            get
            {
                return _facultySimRoles ?? (_facultySimRoles = new List<FacultySimRole>());
            }
            set
            {
                _facultySimRoles = value;
            }
        }

        ICollection<CourseFormat> _courseFormats;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseFormat> CourseFormats
        {
            get
            {
                return _courseFormats ?? (_courseFormats = new List<CourseFormat>());
            }
            set
            {
                _courseFormats = value;
            }
        }
    }
}
