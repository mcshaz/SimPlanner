namespace SP.DataAccess
{
    using Enums;
    using SP.Metadata;
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

        public string BriefDescription { get; set; }

        public string FullDescription { get; set; }

        public Difficulty? Complexity { get; set; }

        public Emersion? EmersionCategory { get; set; }

        public SharingLevel Access { get; set; }

        public string TemplateFilename { get; set; }

        public Guid CourseTypeId { get; set; }
        public Guid DepartmentId { get; set; }

        public virtual CourseType CourseType { get; set; }
        public virtual Department Department { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScenarioResource> ScenarioResources{ get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseSlotScenario> CourseSlotScenarios{ get; set; }
    }
}
