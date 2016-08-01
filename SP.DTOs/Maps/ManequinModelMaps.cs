using SP.DataAccess;
namespace SP.Dto.Maps
{
    internal class ManequinModelMaps: DomainDtoMap<ManequinModel, ManequinModelDto>
    {
        public ManequinModelMaps() : base(m => new ManequinModel
            {
                Id = m.Id,
                Description = m.Description,
                ManufacturerId = m.ManufacturerId
            },
            m => new ManequinModelDto
            {
                Id = m.Id,
                Description = m.Description,
                ManufacturerId = m.ManufacturerId
            })
        { }
    }
}
