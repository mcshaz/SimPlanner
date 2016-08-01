using SP.DataAccess;

namespace SP.Dto.Maps
{
    internal class ScenarioResourceMaps: DomainDtoMap<ScenarioResource, ScenarioResourceDto>
    {
        public ScenarioResourceMaps() : base(m => new ScenarioResource
            {
                Id = m.Id,
                Description = m.Description,
                ResourceFilename = m.ResourceFilename,
                ScenarioId = m.ScenarioId,
                //Scenario = m.Scenario
            },
            m => new ScenarioResourceDto
            {
                Id = m.Id,
                Description = m.Description,
                ResourceFilename = m.ResourceFilename,
                ScenarioId = m.ScenarioId,
                //Scenario = m.Scenario
            })
        { }
    }
}
