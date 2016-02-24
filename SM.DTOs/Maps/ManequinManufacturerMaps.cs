using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.DTOs.Maps
{
    internal static class ManequinManufacturerMaps
    {        internal static Func<ManequinManufacturerDto, ManequinManufacturer>  mapToRepo = m => new ManequinManufacturer {
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
