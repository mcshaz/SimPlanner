using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SP.DataAccess
{
    [MetadataType(typeof(ManikinManufacturerMetadata))]
    public class ManikinManufacturer
    {
        public Guid Id { get; set; }

        public string Name {get; set;}


        public virtual ICollection<ManikinModel> ManikinModels { get; set; }
    }
}
