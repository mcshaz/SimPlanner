using SP.DataAccess;

namespace SP.Dto.Maps
{
    internal class ManikinServiceMaps: DomainDtoMap<ManikinService, ManikinServiceDto>
    {
        public ManikinServiceMaps() : base(m => new ManikinService
            {
                Id = m.Id,
                ManikinId = m.Id,
                PriceEstimate =m.PriceEstimate,
                ProblemDescription =m.ProblemDescription,
                Sent =m.Sent,
                Returned =m.Returned,
                ServiceCost =m.ServiceCost,
                ServicedInternally =m.ServicedInternally
            },
            m => new ManikinServiceDto
            {
                Id = m.Id,
                ManikinId = m.Id,
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
