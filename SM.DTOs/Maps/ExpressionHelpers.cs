using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SM.DTOs.Maps
{
    public static class ExpressionHelpers
    {
        //https://vincentlauzon.com/2011/06/13/expression-trees-part-2-fetching-properties/
        public static IEnumerable<MemberExpression> GetMemberExpressions(Expression e)
        {
            MemberExpression memberExpression;

            while ((memberExpression = e as MemberExpression) != null)
            {
                yield return memberExpression;

                e = memberExpression.Expression;
            }
        }

        public static MemberExpression GetTopExpression(Expression e)
        {
            MemberExpression me;
            MemberExpression lastMe = null;

            while ((me = e as MemberExpression) != null)
            {
                lastMe = me;
                e = me.Expression;
            }
            return lastMe;
        }
        protected override Expression VisitMember(MemberExpression node)
        {
            //    Only relevant if we are trying to access
            //    a member from a class (nullable)
            if (node.Member.DeclaringType.IsClass)
            {    //    We harden the expression we’re about to fetch a member from
                var safeSubExpression = Visit(node.Expression);
                //    Represents the e != null expression
                var test = Expression.NotEqual(
                    safeSubExpression,
                    Expression.Constant(null));
                //    Represents the e!=null ? e.p : null expression
                var inlineIf = Expression.Condition(
                    test,
                    node,
                    Expression.Default(node.Type));

                return inlineIf;
            }

            //    We rely on base implementation otherwise
            return base.VisitMember(node);
        }
    }
}
}
