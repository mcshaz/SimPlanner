using SP.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SP.Dto
{
    [MetadataType(typeof(ResourceSharingInstitutionMetadata))]
    public class ResourceSharingInstitutionDto
    {
        public Guid InstitutionGivingId { get; set; }
        public Guid InstitutionReceivingId { get; set; }

        public InstitutionDto InstitutionGiving { get; set; }
        public InstitutionDto InstitutionReceiving { get; set; }
    }
}
