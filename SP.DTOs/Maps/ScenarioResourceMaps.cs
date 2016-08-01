using SP.DataAccess;
using SP.Dto;
using System;
using System.Linq.Expressions;

namespace SP.Dto.Maps
{
    internal static class ScenarioResourceMaps
    {
        internal static Func<ScenarioResourceDto, ScenarioResource> MapToDomain()
        {
            return m => new ScenarioResource
            {
                Id = m.Id,
                Description = m.Description,
                ResourceFilename = m.ResourceFilename,
                ScenarioId = m.ScenarioId,
                //Scenario = m.Scenario
            };
        }

        internal static Expression<Func<ScenarioResource, ScenarioResourceDto>> MapFromDomain()
        {
            return m => new ScenarioResourceDto
            {
                Id = m.Id,
                Description = m.Description,
                ResourceFilename = m.ResourceFilename,
                ScenarioId = m.ScenarioId,
                //Scenario = m.Scenario
            };
        }
    }
}
