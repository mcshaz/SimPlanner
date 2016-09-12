using SP.DataAccess;
using System;
using System.Linq;
namespace SP.Dto.Maps
{
    internal class HotDrinkMap: DomainDtoMap<HotDrink, HotDrinkDto>
    {
        public HotDrinkMap() : base(m => new HotDrink
            {
                Id = m.Id,
                Description = m.Description
            },
            m => new HotDrinkDto
            {
                Id = m.Id,
                Description = m.Description
            })
        { }

    }
}
