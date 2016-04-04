using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SM.DataAccess
{
    [MetadataType(typeof(ManequinModelMetadata))]
    public class ManequinModel
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public Guid ManufacturerId { get; set; }

        public virtual ManequinManufacturer Manufacturer { get; set; }

        public virtual ICollection<Manequin> Manequins { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Scenario> Scenarios { get; set; }
    }
}
