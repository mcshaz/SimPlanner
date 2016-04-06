using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

//http://stackoverflow.com/questions/35681045/expressiontree-method-to-assign-memberinitexpression-to-property
namespace SM.Dto.Maps
{
    public static class ExpressionTreeExtensions
    {
        public static Expression<Func<T, TMap>> MapNavProperty<T, TMap>(this Expression<Func<T, TMap>> parent, string propName, LambdaExpression nav)
        {
            return MapNavProperty(parent, new[] { new KeyValuePair<string, LambdaExpression>(propName, nav) });
        }
        public static Expression<Func<T, TMap>> MapNavProperty<T, TMap>(this Expression<Func<T, TMap>> parent, IEnumerable<KeyValuePair<string, LambdaExpression>> navs)
        {
            return (Expression<Func<T, TMap>>)MapNavProperty((LambdaExpression)parent, navs);
        }
        static ConstantExpression nullExpression = Expression.Constant(null, typeof(object));
        public static LambdaExpression MapNavProperty(this LambdaExpression parent, IEnumerable<KeyValuePair<string, LambdaExpression>> navs)
        {
            var parentParam = parent.Parameters[0];
            var bindings = new List<MemberBinding>();
            var tMapType = parent.Type.GenericTypeArguments[1];
            foreach (var nav in navs)
            {
                var source = Expression.Property(parentParam, nav.Key);
                var target = tMapType.GetProperty(nav.Key);

                if (target.PropertyType.IsGenericType &&
                   target.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    var selectCall = Expression.Call(typeof(Enumerable), "Select",
                        nav.Value.Type.GenericTypeArguments, source, nav.Value);
                    bindings.Add(Expression.Bind(target, Expression.Call(typeof(Enumerable), "ToList",
                        new[] { nav.Value.Type.GenericTypeArguments[1] }, selectCall)));
                }
                else
                {
                    var navParam = nav.Value.Parameters[0];
                    var mergeVisitor = new ReplaceVisitor(navParam, source);
                    var isNull = Expression.Equal(navParam, nullExpression);
                    var ternary = Expression.Condition(isNull, Expression.Constant(null,nav.Value.Body.Type), nav.Value.Body);
                    var newNavBody = mergeVisitor.Visit(ternary);
                    bindings.Add(Expression.Bind(target, newNavBody));
                }
            }

            var visitor = new AddMemberInitBindingsVisitor(bindings);
            return (LambdaExpression)visitor.Visit(parent);
        }
    }

    public class ReplaceVisitor : ExpressionVisitor
    {
        private readonly Expression _oldExpr;
        private readonly Expression _newExpr;

        public ReplaceVisitor(Expression oldExpr, Expression newExpr)
        {
            _oldExpr = oldExpr;
            _newExpr = newExpr;
        }

        public override Expression Visit(Expression node)
        {
            if (node == _oldExpr)
            {
                return _newExpr;
            }
            return base.Visit(node);
        }
    }

    class AddMemberInitBindingsVisitor : ExpressionVisitor
    {
        public AddMemberInitBindingsVisitor(IEnumerable<MemberBinding> bindings)
        {
            _bindings = bindings;
        }
        private readonly IEnumerable<MemberBinding> _bindings;
        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            return node.Update(node.NewExpression, node.Bindings.Concat(_bindings));
        }
    }

    public class UpdateExpressionVisitor : ExpressionVisitor
    {
        private readonly string _propName;
        private readonly Expression _newExpr;
        public UpdateExpressionVisitor(String propName, Expression newExpr)
        {
            _propName = propName;
            _newExpr = newExpr;
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            if (node.Member.Name == _propName)
            {
                return node.Update(_newExpr);
            }
            return base.VisitMemberAssignment(node);
        }
    }
}

