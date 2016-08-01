using System;
using System.Linq.Expressions;

namespace SP.Dto.Maps
{
    abstract class DomainDtoMap<TDomain, TDto> : IDomainDtoMap<TDomain, TDto>
    {
        protected DomainDtoMap(Func<TDto, TDomain> mapToDomain, Expression<Func<TDomain, TDto>> mapFromDomain)
        {
            MapToDomain = mapToDomain;
            MapFromDomain = mapFromDomain;
        }

        public Func<TDto, TDomain> MapToDomain { get; private set; }
        public Expression<Func<TDomain, TDto>> MapFromDomain { get; private set; }

        public Type TypeofDtoPropertyName(string propertyName)
        {
            return typeof(TDto).GetProperty(propertyName).PropertyType;
        }
    }
}
