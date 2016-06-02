using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

//http://stackoverflow.com/questions/35681045/expressiontree-method-to-assign-memberinitexpression-to-property
namespace SimManager.Tests.Svick
{
    public static class ExpressionTreeExtensions
    {
        public static Expression<Func<T, TMap>> MapNavProperty<T, TMap, U, UMap>(this Expression<Func<T, TMap>> parent, Expression<Func<U, UMap>> nav, string propName)
        {
            var parentParam = parent.Parameters.Single();
            var propExpression = Expression.Property(parentParam, propName);
            var mergeVisitor = new ReplaceVisitor(nav.Parameters.Single(), propExpression);
            var newNavBody = mergeVisitor.Visit(nav.Body);

            var visitor = new UpdateExpressionVisitor(propName, newNavBody);

            return (Expression<Func<T, TMap>>)visitor.Visit(parent);
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

