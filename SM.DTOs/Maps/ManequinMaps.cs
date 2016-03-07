using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;
namespace SM.DTOs.Maps
{
    internal static class ManequinMaps
    {
        internal static Func<ManequinDto, Manequin> mapToRepo()
        {
            return m => new Manequin
            {
                Id = m.Id,
                Description = m.Description,
                DepartmentId = m.DepartmentId,
                ManufacturerId = m.ManufacturerId,
            };
        }

        internal static Expression<Func<Manequin, ManequinDto>> mapFromRepo()
        {
            return m => new ManequinDto
            {
                Id = m.Id,
                Description = m.Description,
                DepartmentId = m.DepartmentId,
                ManufacturerId = m.ManufacturerId,
                //Department = m.Department,
                //Scenarios = m.Scenarios,
                //Manufacturer = m.Manufacturer
            };
        }
    }
}
