using SP.DataAccess.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
namespace SP.Dto
{
    [MetadataType(typeof(CourseParticipantMetadata))]
    public class CourseParticipantDto
	{
        public Guid CourseId { get; set; }
        public bool? IsConfirmed { get; set; }
        public bool IsFaculty { get; set; }
        public bool IsOrganiser { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid ProfessionalRoleId { get; set; }
        public Guid ParticipantId { get; set; }
        public ParticipantDto Participant { get; set; }
        public ProfessionalRoleDto ProfessionalRole { get; set; }
        public DepartmentDto Department { get; set; }
        public CourseDto Course { get; set; }

    }
}
