using SP.DataAccess;

namespace SP.Dto.Maps
{
    internal class CultureMaps : DomainDtoMap<Culture, CultureDto>
    {
        public CultureMaps() : base(m => new Culture
        {
            LocaleCode = m.LocaleCode,
            Name = m.Name,
            CountryCode = m.CountryCode
        },
            m => new CultureDto
            {
                LocaleCode = m.LocaleCode,
                Name = m.Name,
                CountryCode = m.CountryCode
            })
        { }
    }
}
