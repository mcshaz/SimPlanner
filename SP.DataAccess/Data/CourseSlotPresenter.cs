using SP.DataAccess.Data.Interfaces;
using SP.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SP.DataAccess
{
    [MetadataType(typeof(CourseSlotPresenterMetadata))]
    public class CourseSlotPresenter : IModified
    {
        public Guid CourseId { get; set; }
        public Guid CourseSlotId { get; set; }
        public Guid ParticipantId { get; set; }
        public byte StreamNumber { get; set; }
        public DateTime Modified { get; set; }

        public virtual Course Course { get; set; }
        public virtual CourseSlot CourseSlot { get; set; }
        public virtual Participant Participant { get; set; }
    }
}
