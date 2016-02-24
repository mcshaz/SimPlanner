using SM.DataAccess;
using SM.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
namespace SM.Dto
{
    [MetadataType(typeof(ManequinManufacturerMetadata))]
    public class ManequinManufacturerDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        internal static Func<ManequinManufacturerDto, ManequinManufacturer>  mapToRepo = m => new ManequinManufacturer {
            Id = m.Id,
            Name = m.Name
        };

        internal static Expression<Func<ManequinManufacturer, ManequinManufacturerDto>> mapFromRepo= m => new ManequinManufacturerDto
        {
            Id = m.Id,
            Name = m.Name
        };
    }
}
