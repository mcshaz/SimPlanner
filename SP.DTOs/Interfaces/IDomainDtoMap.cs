using SP.Dto.ProcessBreezeRequests;
using System;
using System.Linq.Expressions;

namespace SP.Dto.Maps
{
    interface IDomainDtoMap
    {
        Type TypeofDto { get; }
        Type TypeofServerObject { get; }
        LambdaExpression MapToDto { get; }
        LambdaExpression GetWhereExpression(CurrentPrincipal user);
        object MapFromDto(object dto);
    }

    interface IDomainDtoMap<TDomain, TDto> : IDomainDtoMap
    {

    }
}
