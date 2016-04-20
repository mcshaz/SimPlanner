using SM.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SM.Dto
{
    [MetadataType(typeof(ProfessionalRoleInstitutionMetadata))]
    public class ProfessionalRoleInstitutionDto
    {
        public Guid ProfessionalRoleId { get; set; }
        public Guid InstitutionId { get; set; }

        public virtual ProfessionalRoleDto ProfessionalRole { get; set; }
        public virtual InstitutionDto Institution { get; set; }
    }
}
