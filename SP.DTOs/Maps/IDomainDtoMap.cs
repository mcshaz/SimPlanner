using System;
using System.Linq.Expressions;

namespace SP.Dto.Maps
{
    interface IDomainDtoMap
    {
        Type TypeofDto { get; }
        Type TypeofDomainObject { get; }
        LambdaExpression MapFromDomain { get; }
        object MapToDomain(object dto);
    }

    interface IDomainDtoMap<TDomain, TDto> : IDomainDtoMap
    {

    }
}
