using SP.Dto.ProcessBreezeRequests;
using System;
using System.Linq.Expressions;

namespace SP.Dto.Maps
{
    public abstract class DomainDtoMap<TDomain, TDto> : IDomainDtoMap
    {
        protected DomainDtoMap(Func<TDto, TDomain> mapToDomain, Expression<Func<TDomain, TDto>> mapFromDomain)
        {
            TypedMapToDomain = mapToDomain;
            TypedMapFromDomain = mapFromDomain;
        }

        public Func<TDto, TDomain> TypedMapToDomain { get; private set; }
        public Expression<Func<TDomain, TDto>> TypedMapFromDomain { get; private set; }
        internal Func<CurrentPrincipal,Expression<Func<TDomain,bool>>> WherePredicate { get; set;}

        public LambdaExpression GetWhereExpression(CurrentPrincipal user)
        {
            return WherePredicate == null
                ?null
                :WherePredicate(user);
        }

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

        public Type TypeofServerObject
        {
            get { return typeof(TDomain); }
            //return typeof(TDto).GetProperty(propertyName).PropertyType;
        }
    }
}
