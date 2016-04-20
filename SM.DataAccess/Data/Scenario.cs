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

        public Difficulty? Complexity { get; set; }

        public Emersion? EmersionCategory { get; set; }

        public string TemplateFilename { get; set; }

        public Guid? ManequinModelId { get; set; }
        public Guid CourseTypeId { get; set; }
        public Guid DepartmentId { get; set; }

        public virtual CourseType CourseType { get; set; }
        public virtual Department Department { get; set; }

		ICollection<ScenarioResource> _scenarioResources; 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScenarioResource> ScenarioResources
		{
            get; set;
		}

		ICollection<CourseSlotScenario> _courseSlotScenarios; 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseSlotScenario> CourseSlotScenarios
        {
            get; set;
		}
    }
}
