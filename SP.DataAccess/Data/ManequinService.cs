using SP.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SP.DataAccess
{
    [MetadataType(typeof(ManequinServiceMetadata))]
    public class ManequinService
    {
        public Guid Id { get; set; }

        public string ProblemDescription { get; set; }

        public bool ServicedInternally { get; set; }

        public DateTime Sent { get; set; }

        public DateTime? Returned { get; set; }

        public decimal? PriceEstimate { get; set; }

        public decimal ServiceCost { get; set; }

        public Guid ManequinId { get; set; }

        public virtual Manequin Manequin { get; set; }
    }
}
