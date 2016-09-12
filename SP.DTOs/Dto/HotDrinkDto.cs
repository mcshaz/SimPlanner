using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SP.Dto
{
    [MetadataType(typeof(HotDrinkMetadata))]
    public class HotDrinkDto
    {
        public Guid Id { get;  set; }
        public string Description { get; set; }

        public virtual ICollection<ParticipantDto> Participants { get; set; }
    }
}
