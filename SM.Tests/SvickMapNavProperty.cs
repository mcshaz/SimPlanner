using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

//http://stackoverflow.com/questions/35681045/expressiontree-method-to-assign-memberinitexpression-to-property
namespace SimManager.Tests.Svick
{
    public static class ExpressionTreeExtensions
    {
        public static Expression<Func<T, TMap>> MapNavProperty<T, TMap, U, UMap>(this Expression<Func<T, TMap>> parent, Expression<Func<U, UMap>> nav, string propName)
        {
            var parentParam = parent.Parameters[0];
            var source = Expression.Property(parentParam, propName);
            var target = typeof(TMap).GetProperty(propName);
            MemberBinding binding;
            if (target.IsICollection())
            {
                binding = Expression.Bind(target, Expression.Call(
                    typeof(Enumerable), "Select", nav.Type.GenericTypeArguments, source, nav));
            }
            else
            {
                var mergeVisitor = new ReplaceVisitor(nav.Parameters[0], source);
                var newNavBody = mergeVisitor.Visit(nav.Body);
                binding = Expression.Bind(target, newNavBody);
            }
            var visitor = new AddMemberInitBindingsVisitor(binding);
            var returnVar = visitor.Visit(parent);
            return (Expression<Func<T, TMap>>)returnVar;
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
        public AddMemberInitBindingsVisitor(params MemberBinding[] bindings)
        {
            _bindings = bindings;
        }
        private readonly MemberBinding[] _bindings;
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

    internal static class PropertyInfoHelpers
    {
        public static bool IsICollection(this PropertyInfo pi)
        {
            //debugging hack todo - remove
            return IsIEnumerableGeneric(pi);
            return pi.PropertyType.IsGenericType &&
                   pi.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>);
        }
        public static bool IsIEnumerableGeneric(this PropertyInfo pi)
        {
            return pi.PropertyType.IsGenericType &&
                pi.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }
        public static bool ImplementsEnumerableGeneric(this PropertyInfo pi)
        {
            return pi.PropertyType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition()== typeof(IEnumerable<>));
        }
    }
}

