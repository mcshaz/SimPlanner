using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.Dto.Maps
{
    internal static class ManequinModelMaps
    {
        internal static Func<ManequinModelDto, ManequinModel> mapToRepo()
        {
            return m => new ManequinModel
            {
                Id = m.Id,
                Description = m.Description,
                ManufacturerId = m.ManufacturerId
            };
        }

        internal static Expression<Func<ManequinModel, ManequinModelDto>> mapFromRepo()
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
