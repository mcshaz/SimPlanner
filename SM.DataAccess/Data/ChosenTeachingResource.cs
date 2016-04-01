using SM.DataAccess.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SM.DataAccess
{
    [MetadataType(typeof(ChosenTeachingResourceMetadata))]
    public class ChosenTeachingResource
    {
        public Guid CourseId { get; set; }
        public Guid CourseSlotId { get; set; }
        public Guid ActivityTeachingResourceId { get; set; }

        public Course Course { get; set; }
        public CourseSlot CourseSlot { get; set; }
        public ActivityTeachingResource ActivityTeachingResource { get; set; }
    }
}
