using SP.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SP.DataAccess
{
    [MetadataType(typeof(ManikinServiceMetadata))]
    public class ManikinService
    {
        public Guid Id { get; set; }

        public string ProblemDescription { get; set; }

        public bool ServicedInternally { get; set; }

        public DateTime Sent { get; set; }

        public DateTime? Returned { get; set; }

        public decimal? PriceEstimate { get; set; }

        public decimal? ServiceCost { get; set; }

        public Guid ManikinId { get; set; }

        public virtual Manikin Manikin { get; set; }
    }
}
