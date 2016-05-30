using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SM.Metadata
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
    }
}
