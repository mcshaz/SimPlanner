using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SM.Dto
{
    [MetadataType(typeof(ManequinManufacturerMetadata))]
    public class ManequinManufacturerDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<ManequinDto> Manequins { get; set; }

    }
}
