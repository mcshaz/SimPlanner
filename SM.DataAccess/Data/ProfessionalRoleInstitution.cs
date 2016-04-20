using SM.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SM.DataAccess
{
    [MetadataType(typeof(ProfessionalRoleInstitutionMetadata))]
    public class ProfessionalRoleInstitution
    {
        public Guid ProfessionalRoleId { get; set; }
        public Guid InstitutionId { get; set; }

        public virtual ProfessionalRole ProfessionalRole { get; set; }
        public virtual Institution Institution { get; set; }
    }
}
