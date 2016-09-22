using SP.DataAccess;
namespace SP.Dto.Maps
{
    internal class ManikinMaps: DomainDtoMap<Manikin, ManikinDto>
    {
        public ManikinMaps() : base(m => new Manikin
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
            m => new ManikinDto
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
