using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SP.DataAccess
{
    [MetadataType(typeof(ManequinManufacturerMetadata))]
    public class ManequinManufacturer
    {
        public Guid Id { get; set; }

        public string Name {get; set;}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ManequinModel> ManequinModels { get; set; }
    }
}
