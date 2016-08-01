using System;
using System.Linq.Expressions;

namespace SP.Dto.Maps
{
    abstract class DomainDtoMap<TDomain, TDto> : IDomainDtoMap
    {
        protected DomainDtoMap(Func<TDto, TDomain> mapToDomain, Expression<Func<TDomain, TDto>> mapFromDomain)
        {
            TypedMapToDomain = mapToDomain;
            TypedMapFromDomain = mapFromDomain;
        }

        public Func<TDto, TDomain> TypedMapToDomain { get; private set; }
        public Expression<Func<TDomain, TDto>> TypedMapFromDomain { get; private set; }

        public LambdaExpression MapToDto { get { return TypedMapFromDomain; } }
        public object MapFromDto(object dto)
        {
            return TypedMapToDomain((TDto)dto);
        }

        public Type TypeofDto
        {
            get { return typeof(TDto); }
            //return typeof(TDto).GetProperty(propertyName).PropertyType;
        }

        public Type TypeofDomainObject
        {
            get { return typeof(TDomain); }
            //return typeof(TDto).GetProperty(propertyName).PropertyType;
        }
    }
}
