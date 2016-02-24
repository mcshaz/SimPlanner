using SM.DataAccess;
using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
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

        internal static Func<CourseTeachingResourceDto, CourseTeachingResource>  mapToRepo = m => new CourseTeachingResource {
            Id = m.Id,
            Name = m.Name,
            ResourceFilename = m.ResourceFilename,
            CourseSlotId = m.CourseSlotId,
            //CourseSlot = m.CourseSlot
        };

        internal static Expression<Func<CourseTeachingResource, CourseTeachingResourceDto>> mapFromRepo= m => new CourseTeachingResourceDto
        {
            Id = m.Id,
            Name = m.Name,
            ResourceFilename = m.ResourceFilename,
            CourseSlotId = m.CourseSlotId,
            //CourseSlot = m.CourseSlot
        };
	}
    [MetadataType(typeof(ResourceMetadata))]
    public class ScenarioResourceDto : ResourceDto
    {
        public Guid ScenarioId { get; set; }
        public ScenarioDto Scenario { get; set; }

        internal static Func<ScenarioResourceDto, ScenarioResource>  mapToRepo = m => new ScenarioResource {
            Id = m.Id,
            Name = m.Name,
            ResourceFilename = m.ResourceFilename,
            ScenarioId = m.ScenarioId,
            //Scenario = m.Scenario
        };

        internal static Expression<Func<ScenarioResource, ScenarioResourceDto>> mapFromRepo= m => new ScenarioResourceDto
        {
            Id = m.Id,
            Name = m.Name,
            ResourceFilename = m.ResourceFilename,
            ScenarioId = m.ScenarioId,
            //Scenario = m.Scenario
        };
    }
}
