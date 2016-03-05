using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

//http://stackoverflow.com/questions/35681045/expressiontree-method-to-assign-memberinitexpression-to-property
namespace SimManager.Tests.IvanStoev
{
    public static class ExpressionTreeExtensions
    {
        //change name to insertNavProperty
        public static Expression<Func<T, TMap>> MapNavProperty<T, TMap, U, UMap>(this Expression<Func<T, TMap>> parent, Expression<Func<U, UMap>> nav, string propName)
        {
            var parameter = parent.Parameters[0];
            var body = parent.Body.ReplaceMemberAssignment(
                typeof(TMap).GetProperty(propName),
                nav.ReplaceParameter(Expression.Property(parameter, propName))
            );
            return Expression.Lambda<Func<T, TMap>>(body, parameter);
        }

        static Expression ReplaceParameter(this LambdaExpression lambda, Expression target)
        {
            return lambda.Body.ReplaceParameter(lambda.Parameters.Single(), target);
        }

        static Expression ReplaceParameter(this Expression expression, ParameterExpression source, Expression target)
        {
            return new ParameterReplacer { Source = source, Target = target }.Visit(expression);
        }

        static Expression InsertParameter(this LambdaExpression lambda, Expression target)
        {
            return lambda.Body.InsertParameter(lambda.Parameters.Single(), target);
        }

        static Expression InsertParameter(this Expression expression, ParameterExpression source, Expression target)
        {
            return new ParameterReplacer { Source = source, Target = target }.Visit(expression);
        }

        static Expression ReplaceMemberAssignment(this Expression expression, MemberInfo member, Expression value)
        {
            return new MemberAssignmentReplacer { Member = member, Value = value }.Visit(expression);
        }
    }

    class ParameterReplacer : ExpressionVisitor
    {
        public ParameterExpression Source;
        public Expression Target;
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == Source ? Target : base.VisitParameter(node);
        }
    }

    class MemberAssignmentReplacer : ExpressionVisitor
    {
        public MemberInfo Member;
        public Expression Value;
        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            return node.Member == Member ? node.Update(Value) : base.VisitMemberAssignment(node);
        }
    }

    public class AddBindingVisitor<TMap> : ExpressionVisitor
    {
        private string _propNamr;
        private ParameterExpression _newBinding;
        private ParameterExpression _existingParam;

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            _existingParam = node.Parameters.Single();
            return base.VisitLambda(node);
        }

        AddBindingVisitor(string propName, ParameterExpression newBinding)
        {
            _propNamr = propName;
            _newBinding = newBinding;
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            var member = typeof(TMap).GetProperty(_propNamr);
            var newBindings = new[]
            {
                Expression.Bind(member, Expression.Property(_existingParam, _propNamr)),
            };
            var updatedNode = node.Update(
                node.NewExpression,
                node.Bindings.Concat(newBindings));
            return base.VisitMemberInit(updatedNode);
        }
    }
}
