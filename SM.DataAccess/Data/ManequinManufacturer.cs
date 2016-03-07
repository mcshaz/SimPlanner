using SM.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SM.DataAccess
{
    [MetadataType(typeof(ManequinManufacturerMetadata))]
    public class ManequinManufacturer
    {
        public Guid Id { get; set; }

        public string Name {get; set;}
    }
}
