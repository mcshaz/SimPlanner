using SP.DataAccess;
using SP.Dto;
using System;
using System.Linq.Expressions;
namespace SP.Dto.Maps
{
    internal static class ManequinModelMaps
    {
        internal static Func<ManequinModelDto, ManequinModel> MapToDomain()
        {
            return m => new ManequinModel
            {
                Id = m.Id,
                Description = m.Description,
                ManufacturerId = m.ManufacturerId
            };
        }

        internal static Expression<Func<ManequinModel, ManequinModelDto>> MapFromDomain()
        {
            return m => new ManequinModelDto
            {
                Id = m.Id,
                Description = m.Description,
                ManufacturerId = m.ManufacturerId
            };
        }
    }
}
