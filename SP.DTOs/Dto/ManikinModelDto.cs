using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SP.Dto
{
    [MetadataType(typeof(ManikinModelMetadata))]
    public class ManikinModelDto
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public Guid ManufacturerId { get; set; }

        public virtual ManikinManufacturerDto Manufacturer { get; set; }

        public virtual ICollection<ManikinDto> Manikins { get; set; }
    }
}
