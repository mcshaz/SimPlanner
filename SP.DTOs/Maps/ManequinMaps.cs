using SP.DataAccess;
using SP.Dto;
using System;
using System.Linq.Expressions;
namespace SP.Dto.Maps
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
                ModelId = m.ModelId,
                DecommissionDate = m.DecommissionDate,
                DecommissionReason = m.DecommissionReason,
                PurchaseDate = m.PurchaseDate,
                PurchasedNew = m.PurchasedNew,
                LocalCurrencyPurchasePrice = m.LocalCurrencyPurchasePrice
            };
        }

        internal static Expression<Func<Manequin, ManequinDto>> mapFromRepo()
        {
            return m => new ManequinDto
            {
                Id = m.Id,
                Description = m.Description,
                DepartmentId = m.DepartmentId,
                ModelId = m.ModelId,
                DecommissionDate = m.DecommissionDate,
                DecommissionReason = m.DecommissionReason,
                PurchaseDate = m.PurchaseDate,
                PurchasedNew = m.PurchasedNew
                //Department = m.Department,
                //Scenarios = m.Scenarios,
                //Manufacturer = m.Manufacturer
            };
        }
    }
}
