using SM.DataAccess.Enums;
using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SM.Dto
{
    [MetadataType(typeof(ProfessionalRoleMetadata))]
    public class ProfessionalRoleDto
	{
        public Guid Id { get; set; }
        public string Description { get; set; }
        public ProfessionalCategory Category { get; set; }
        public virtual ICollection<ParticipantDto> Participants { get; set; }
        public virtual ICollection<CountryDto> Countries { get; set; }

    }
}
