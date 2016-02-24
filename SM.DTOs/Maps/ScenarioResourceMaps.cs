using SM.DataAccess;
using SM.Dto;
using System;
using System.Linq.Expressions;

namespace SM.DTOs.Maps
{
    internal static class ScenarioResourceMaps
    {
        internal static Func<ScenarioResourceDto, ScenarioResource> mapToRepo = m => new ScenarioResource
        {
            Id = m.Id,
            Name = m.Name,
            ResourceFilename = m.ResourceFilename,
            ScenarioId = m.ScenarioId,
            //Scenario = m.Scenario
        };

        internal static Expression<Func<ScenarioResource, ScenarioResourceDto>> mapFromRepo = m => new ScenarioResourceDto
        {
            Id = m.Id,
            Name = m.Name,
            ResourceFilename = m.ResourceFilename,
            ScenarioId = m.ScenarioId,
            //Scenario = m.Scenario
        };
    }
}
