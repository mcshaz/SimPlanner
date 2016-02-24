using SM.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SM.DataAccess
{
    public abstract class Resource
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ResourceFilename { get; set; }
    }
    [MetadataType(typeof(ResourceMetadata))]
    public class CourseTeachingResource : Resource
    {
        public Guid CourseSlotId { get; set; }
        public virtual CourseSlot CourseSlot { get; set; }
    }

    [MetadataType(typeof(ResourceMetadata))]
    public class ScenarioResource : Resource
    {
        public Guid ScenarioId { get; set; }
        public virtual Scenario Scenario { get; set; }
    }
}
