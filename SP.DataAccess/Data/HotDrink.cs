using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SP.DataAccess
{
    [MetadataType(typeof(HotDrinkMetadata))]
    public class HotDrink
    {
        public Guid Id { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Participant> Participants { get; set; }
    }
}
