using SM.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SM.Dto
{
    [MetadataType(typeof(ManequinServiceMetadata))]
    public class ManequinServiceDto
    {
        public Guid Id { get; set; }

        public string ProblemDescription { get; set; }

        public bool ServicedInternally { get; set; }

        public DateTime Sent { get; set; }

        public DateTime? Returned { get; set; }

        public decimal? PriceEstimate { get; set; }

        public decimal ServiceCost { get; set; }

        public Guid ManequinId { get; set; }

        public virtual ManequinDto Manequin { get; set; }
    }
}
