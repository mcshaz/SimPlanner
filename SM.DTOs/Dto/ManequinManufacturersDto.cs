using SM.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
namespace SM.Dto
{
    [MetadataType(typeof(ManequinManufacturerMetadata))]
    public class ManequinManufacturerDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

    }
}
