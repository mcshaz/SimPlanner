using SP.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SP.DataAccess
{
    [MetadataType(typeof(ResourceSharingInstitutionMetadata))]
    public class ResourceSharingInstitution
    {
        public Guid InstitutionGivingId { get; set; }
        public Guid InstitutionReceivingId { get; set; }

        public Institution InstitutionGiving { get; set; }
        public Institution InstitutionReceiving { get; set; }
    }
}
