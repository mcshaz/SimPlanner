using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SimManager.Tests
{
    public class MergingVisitor<TSource, TBaseDest, TExtendedDest> : ExpressionVisitor
    {
        /// <summary>
        /// Internal helper, that rebinds the lambda of the base init expression. The
        /// reason for this is that the member initialization list of the base expression
        /// is bound to the range variable in the base expression. To be able to merge those
        /// into the extended expression, all those references have to be rebound to the
        /// range variable of the extended expression. That rebinding is done by this helper.
        /// </summary>

        private MemberInitExpression baseInit;
        private ParameterExpression baseParameter;
        private ParameterExpression newParameter;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="baseExpression">The base expression to merge
        /// into the member init list of the extended expression.</param>
        public MergingVisitor(Expression<Func<TSource, TBaseDest>> baseExpression)
        {
            var lambda = (LambdaExpression)baseExpression;
            baseInit = (MemberInitExpression)lambda.Body;

            baseParameter = lambda.Parameters[0];
        }

        /// <summary>
        /// Pick up the extended expressions range variable.
        /// </summary>
        /// <typeparam name="T">Not used</typeparam>
        /// <param name="node">Lambda expression node</param>
        /// <returns>Unmodified expression tree</returns>
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (newParameter == null)
            {
                newParameter = node.Parameters[0];
            }
            return base.VisitLambda<T>(node);
        }

        /// <summary>
        /// Visit the member init expression of the extended expression. Merge the base 
        /// expression into it.
        /// </summary>
        /// <param name="node">Member init expression node.</param>
        /// <returns>Merged member init expression.</returns>
        protected override Expression VisitMemberInit(MemberInitExpression node)
        {

            MemberInitExpression reboundBaseInit = null;

            var mergedInitList = node.Bindings.Concat(reboundBaseInit.Bindings);

            return Expression.MemberInit(Expression.New(typeof(TExtendedDest)),
                mergedInitList);
        }
    }
}
