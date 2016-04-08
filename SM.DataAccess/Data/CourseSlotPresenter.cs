using SM.DataAccess.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SM.DataAccess
{
    [MetadataType(typeof(CourseSlotPresenterMetadata))]
    public class CourseSlotPresenter
    {
        public Guid CourseId { get; set; }

        public Guid CourseSlotId { get; set; }

        public Guid ParticipantId { get; set; }

        public virtual Course Course { get; set; }
        public virtual CourseSlot CourseSlot { get; set; }
        public virtual Participant Participant { get; set; }
    }
}
