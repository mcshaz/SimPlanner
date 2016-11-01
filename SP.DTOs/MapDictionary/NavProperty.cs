using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SP.Dto.Maps
{
    public class NavProperty
    {
        public NavProperty(PropertyInfo propertyInfo, LambdaExpression select)
        {
            PropertyInfo = propertyInfo;
            Select = select;
        }
        public PropertyInfo PropertyInfo { get; private set; }
        public LambdaExpression Select { get; set; }
        public LambdaExpression Where { get; set; }
        bool? _isICollection;
        public bool IsICollection {
            get {
                return (_isICollection ?? (_isICollection = PropertyInfo.PropertyType.IsGenericType &&
                   PropertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))).Value;
            }
        }
    }

    public class NavProperty<T,TMap> : NavProperty
    {
        public NavProperty(string propertyName, Expression<Func<T,TMap>> select) : base(typeof(TMap).GetProperty(propertyName), select) { }
    }
}
