using System;
using System.Linq.Expressions;

namespace SM.DTOs.Maps
{
    public abstract class IDtoMap
    {
        string Name { get; set; }
        LambdaExpression Map { get; set; }
    }
    public class DtoMap<T,TMap> : IDtoMap
    {
        public string Name { get; set; }
        public Expression<Func<T,TMap>> Map { get; set; }
    }
}
