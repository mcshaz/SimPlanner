using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SM.Dto
{
    [MetadataType(typeof(ManequinModelMetadata))]
    public class ManequinModelDto
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public Guid ManufacturerId { get; set; }

        public virtual ManequinManufacturerDto Manufacturer { get; set; }

        public virtual ICollection<ManequinDto> Manequins { get; set; }

        public virtual ICollection<ScenarioDto> Scenarios { get; set; }
    }
}
