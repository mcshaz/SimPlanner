using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

//http://stackoverflow.com/questions/35681045/expressiontree-method-to-assign-memberinitexpression-to-property
namespace SimManager.Tests.IvanStoev
{
    public static class ExpressionTreeExtensions
    {

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
}
