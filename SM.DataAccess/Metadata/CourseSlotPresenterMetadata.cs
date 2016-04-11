using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SM.Metadata
{
    public class CourseSlotPresenterMetadata
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid CourseId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid CourseSlotId { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid ParticipantId { get; set; }

        [Range(0, CourseSlotMetadata.MaxStreams)]
        public byte StreamNumber { get; set; }
    }
}
