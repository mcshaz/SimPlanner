using SP.DataAccess;

namespace SP.Dto.Maps
{
    internal class ScenarioResourceMaps: DomainDtoMap<ScenarioResource, ScenarioResourceDto>
    {
        public ScenarioResourceMaps() : base(m => new ScenarioResource
            {
                Id = m.Id,
                Description = m.Description,
                FileName = m.FileName,
                ScenarioId = m.ScenarioId,
                FileModified = m.FileModified,
                FileSize = m.FileSize,
            },
            m => new ScenarioResourceDto
            {
                Id = m.Id,
                Description = m.Description,
                FileName = m.FileName,
                ScenarioId = m.ScenarioId,
                FileModified = m.FileModified,
                FileSize = m.FileSize
            })
        { }
    }
}
