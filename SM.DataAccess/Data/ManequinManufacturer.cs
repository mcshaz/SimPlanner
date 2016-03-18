using SM.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SM.DataAccess
{
    [MetadataType(typeof(ManequinManufacturerMetadata))]
    public class ManequinManufacturer
    {
        public Guid Id { get; set; }

        public string Name {get; set;}

        ICollection<Manequin> _manequins;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Manequin> Manequins
        {
            get
            {
                return _manequins ?? (_manequins = new List<Manequin>());
            }
            set
            {
                _manequins = value;
            }
        }
    }
}
