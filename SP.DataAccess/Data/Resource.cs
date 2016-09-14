using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SP.DataAccess
{
    [MetadataType(typeof(ResourceMetadata))]
    public abstract class Resource
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public DateTime? FileModified { get; set; }
        public long? FileSize { get; set; }
    }
    [MetadataType(typeof(ResourceMetadata))]
    public class ActivityTeachingResource : Resource
    {
        public Guid CourseActivityId { get; set; }
        public virtual CourseActivity CourseActivity { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChosenTeachingResource> ChosenTeachingResources { get; set; }
    }

    [MetadataType(typeof(ResourceMetadata))]
    public class ScenarioResource : Resource
    {
        public Guid ScenarioId { get; set; }
        public virtual Scenario Scenario { get; set; }
    }
}
