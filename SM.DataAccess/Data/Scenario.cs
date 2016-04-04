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

        public Difficulty Complexity { get; set; }

        public Emersion? EmersionCategory { get; set; }

        public string TemplateFilename { get; set; }

        public Guid? ManequinModelId { get; set; }
        public Guid CourseTypeId { get; set; }
        public Guid DepartmentId { get; set; }

        public virtual ManequinModel ManequinModel { get; set; }
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

		ICollection<ScenarioResource> _scenarioResources; 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScenarioResource> ScenarioResources
		{
			get
			{
				return _scenarioResources ?? (_scenarioResources = new List<ScenarioResource>());
			}
			set
			{
                _scenarioResources = value;
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
    }
}
