using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SP.DataAccess
{
    [MetadataType(typeof(ManikinModelMetadata))]
    public class ManikinModel
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public Guid ManufacturerId { get; set; }

        public virtual ManikinManufacturer Manufacturer { get; set; }

        public virtual ICollection<Manikin> Manikins { get; set; }

    }
}
