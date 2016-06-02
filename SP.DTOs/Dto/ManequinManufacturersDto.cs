using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SP.Dto
{
    [MetadataType(typeof(ManequinManufacturerMetadata))]
    public class ManequinManufacturerDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<ManequinModelDto> ManequinModels { get; set; }

    }
}
