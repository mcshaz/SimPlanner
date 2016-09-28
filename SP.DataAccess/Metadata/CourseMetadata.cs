using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Metadata
{    
	public class CourseMetadata
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        [StringLength(256)]
        public string ParticipantVideoFilename { get; set; }

        [StringLength(256)]
        public string FeedbackSummaryFilename { get; set; }

        [Range(0,480)]
        public int? FacultyMeetingDuration { get; set; }

        [DisplayName("Course Start")]
        public DateTime StartUtc { get; set; }
    }
}
