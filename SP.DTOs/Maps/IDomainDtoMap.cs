using System;
using System.Linq.Expressions;

namespace SP.Dto.Maps
{
    interface IDomainDtoMap<TDomain, TDto>
    {
        Func<TDto, TDomain> MapToDomain { get; }
        Expression<Func<TDomain, TDto>> MapFromDomain { get; }

        Type TypeofDtoPropertyName(string propertyName);
    }
}
