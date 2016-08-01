using SP.DataAccess;
namespace SP.Dto.Maps
{
    internal class ManequinMaps: DomainDtoMap<Manequin, ManequinDto>
    {
        public ManequinMaps() : base(m => new Manequin
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
            },
            m => new ManequinDto
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
            })
        { }
    }
}
