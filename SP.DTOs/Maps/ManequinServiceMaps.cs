using SP.DataAccess;
using SP.Dto;
using System;
using System.Linq.Expressions;
namespace SP.Dto.Maps
{
    internal static class ManequinServiceMaps
    {
        internal static Func<ManequinServiceDto, ManequinService> mapToRepo()
        {
            return m => new ManequinService
            {
                Id = m.Id,
                ManequinId = m.Id,
                PriceEstimate =m.PriceEstimate,
                ProblemDescription =m.ProblemDescription,
                Sent =m.Sent,
                Returned =m.Returned,
                ServiceCost =m.ServiceCost,
                ServicedInternally =m.ServicedInternally
            };
        }

        internal static Expression<Func<ManequinService, ManequinServiceDto>> mapFromRepo()
        {
            return m => new ManequinServiceDto
            {
                Id = m.Id,
                ManequinId = m.Id,
                PriceEstimate = m.PriceEstimate,
                ProblemDescription = m.ProblemDescription,
                Sent = m.Sent,
                Returned = m.Returned,
                ServiceCost = m.ServiceCost,
                ServicedInternally = m.ServicedInternally
            };
        }
    }
}
