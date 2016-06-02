using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Metadata
{
    public class ManequinServiceMetadata
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        [StringLength(1028)]
        public string ProblemDescription { get; set; }

        [Column(TypeName = "date")]
        public DateTime Sent { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Returned { get; set; }

        [Column(TypeName = "money")]
        public decimal? PriceEstimate { get; set; }

        [Column(TypeName = "money")]
        public decimal ServiceCost { get; set; }
    }
}
