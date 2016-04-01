using SM.DataAccess.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SM.Dto
{
    [MetadataType(typeof(ChosenTeachingResourceMetadata))]
    public class ChosenTeachingResourceDto
    {
        public Guid CourseId { get; set; }
        public Guid CourseSlotId { get; set; }
        public Guid ActivityTeachingResourceId { get; set; }

        public CourseDto Course { get; set; }
        public CourseSlotDto CourseSlot { get; set; }
        public ActivityTeachingResourceDto ActivityTeachingResource { get; set; }
    }
}
