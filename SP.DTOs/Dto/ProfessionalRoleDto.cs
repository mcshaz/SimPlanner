using SP.DataAccess.Enums;
using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SP.Dto
{
    [MetadataType(typeof(ProfessionalRoleMetadata))]
    public class ProfessionalRoleDto
	{
        public Guid Id { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public ProfessionalCategory Category { get; set; }
        public virtual ICollection<ParticipantDto> Participants { get; set; }
        public virtual ICollection<ProfessionalRoleInstitutionDto> ProfessionalRoleInstitutions { get; set; }
        public virtual ICollection<CourseParticipantDto> CourseParticipants { get; set; }
    }
}
