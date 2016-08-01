using SP.DataAccess;
namespace SP.Dto.Maps
{
    internal class ManequinManufacturerMaps: DomainDtoMap<ManequinManufacturer, ManequinManufacturerDto>
    {
        public ManequinManufacturerMaps() : base(m => new ManequinManufacturer
            {
                Id = m.Id,
                Name = m.Name
            },
            m => new ManequinManufacturerDto
            {
                Id = m.Id,
                Name = m.Name
            })
        { }
    }
}
