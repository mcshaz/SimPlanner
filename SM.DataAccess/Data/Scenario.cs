namespace SM.DataAccess
{
    using Enums;
    using SM.Metadata;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [MetadataType(typeof(ScenarioMetadata))]
    public class Scenario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string Description { get; set; }

        public Guid DepartmentId { get; set; }

        public Difficulty Complexity { get; set; }

        public Emersion? EmersionCategory { get; set; }

        public string TemplateFilename { get; set; }

        public Guid? ManequinId { get; set; }

        public Guid CourseTypeId { get; set; }

        public virtual Manequin Manequin { get; set; }

        public virtual CourseType CourseType { get; set; }

        public virtual Department Department { get; set; }

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

		ICollection<ScenarioResource> _resources; 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScenarioResource> Resources
		{
			get
			{
				return _resources ?? (_resources = new List<ScenarioResource>());
			}
			set
			{
				_resources = value;
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
