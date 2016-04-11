using SM.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
namespace SM.Dto
{
    [MetadataType(typeof(CourseSlotPresenterMetadata))]
    public class CourseSlotPresenterDto
    {
        public Guid CourseId { get; set; }
        public Guid CourseSlotId { get; set; }
        public Guid ParticipantId { get; set; }
        public byte StreamNumber { get; set; }
        public CourseDto Course { get; set; }
        public CourseSlotDto CourseSlot { get; set; }
        public ParticipantDto Participant { get; set; }

    }
}
