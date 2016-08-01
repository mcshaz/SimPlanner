using SP.DataAccess;
namespace SP.Dto.Maps
{
    internal class ManequinServiceMaps: DomainDtoMap<ManequinService, ManequinServiceDto>
    {
        public ManequinServiceMaps() : base(m => new ManequinService
            {
                Id = m.Id,
                ManequinId = m.Id,
                PriceEstimate =m.PriceEstimate,
                ProblemDescription =m.ProblemDescription,
                Sent =m.Sent,
                Returned =m.Returned,
                ServiceCost =m.ServiceCost,
                ServicedInternally =m.ServicedInternally
            },
            m => new ManequinServiceDto
            {
                Id = m.Id,
                ManequinId = m.Id,
                PriceEstimate = m.PriceEstimate,
                ProblemDescription = m.ProblemDescription,
                Sent = m.Sent,
                Returned = m.Returned,
                ServiceCost = m.ServiceCost,
                ServicedInternally = m.ServicedInternally
            })
        { }
    }
}
