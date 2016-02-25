using SM.DataAccess.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
namespace SM.Dto
{
    [MetadataType(typeof(CourseParticipantMetadata))]
    public class CourseParticipantDto
	{
        public Guid ParticipantId { get; set; }
        public Guid CourseId { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsFaculty { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid ProfessionalRoleId { get; set; }
        public ParticipantDto Participant { get; set; }
        public CourseDto Course { get; set; }

    }
}
