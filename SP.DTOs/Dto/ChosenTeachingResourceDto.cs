using SP.DataAccess.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SP.Dto
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
