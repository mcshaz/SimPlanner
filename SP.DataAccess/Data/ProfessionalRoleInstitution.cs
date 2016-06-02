using SP.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SP.DataAccess
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
