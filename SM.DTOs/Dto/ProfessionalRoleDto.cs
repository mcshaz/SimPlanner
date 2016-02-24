using SM.DataAccess;
using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
namespace SM.Dto
{
    [MetadataType(typeof(ProfessionalRoleMetadata))]
    public class ProfessionalRoleDto
	{
        public Guid Id { get; set; }
        public string Description { get; set; }
        public ProfessionalCategory Category { get; set; }
        public ICollection<ParticipantDto> Participants { get; set; }
        public ICollection<CountryDto> Countries { get; set; }

        internal static Func<ProfessionalRoleDto, ProfessionalRole>  mapToRepo = m => new ProfessionalRole {
            Id = m.Id,
            Description = m.Description,
            Category = m.Category,
            //Participants = m.Participants,
            //Countries = m.Countries
        };

        internal static Expression<Func<ProfessionalRole, ProfessionalRoleDto>> mapFromRepo= m => new ProfessionalRoleDto
        {
            Id = m.Id,
            Description = m.Description,
            Category = m.Category,
            //Participants = m.Participants,
            //Countries = m.Countries
        };
    }
}
