using SP.DataAccess;

namespace SP.Dto.Maps
{
    internal class ManikinModelMaps: DomainDtoMap<ManikinModel, ManikinModelDto>
    {
        public ManikinModelMaps() : base(m => new ManikinModel
            {
                Id = m.Id,
                Description = m.Description,
                ManufacturerId = m.ManufacturerId
            },
            m => new ManikinModelDto
            {
                Id = m.Id,
                Description = m.Description,
                ManufacturerId = m.ManufacturerId
            })
        { }
    }
}
