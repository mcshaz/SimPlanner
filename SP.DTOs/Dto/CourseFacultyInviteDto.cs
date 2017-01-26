using SP.DataAccess.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
namespace SP.Dto
{
    [MetadataType(typeof(CourseParticipantMetadata))]
    public class CourseFacultyInviteDto
	{
        public Guid CourseId { get; set; }
        public Guid ParticipantId { get; set; }

        public ParticipantDto Faculty { get; set; }
        public CourseDto Course { get; set; }
    }
}
