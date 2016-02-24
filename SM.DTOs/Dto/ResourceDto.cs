using SM.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
namespace SM.Dto
{
    public abstract class ResourceDto 
	{
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ResourceFilename { get; set; }
	}
    [MetadataType(typeof(ResourceMetadata))]
    public class CourseTeachingResourceDto : ResourceDto
    {
        public Guid CourseSlotId { get; set; }
        public CourseSlotDto CourseSlot { get; set; }

	}
    [MetadataType(typeof(ResourceMetadata))]
    public class ScenarioResourceDto : ResourceDto
    {
        public Guid ScenarioId { get; set; }
        public ScenarioDto Scenario { get; set; }
    }
}
