using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SP.DataAccess
{
    [MetadataType(typeof(ManequinModelMetadata))]
    public class ManequinModel
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public Guid ManufacturerId { get; set; }

        public virtual ManequinManufacturer Manufacturer { get; set; }

        public virtual ICollection<Manequin> Manequins { get; set; }

    }
}
