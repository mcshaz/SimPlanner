using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SP.Dto
{
    [MetadataType(typeof(ManikinManufacturerMetadata))]
    public class ManikinManufacturerDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<ManikinModelDto> ManikinModels { get; set; }

    }
}
