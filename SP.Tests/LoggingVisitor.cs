using System;
using System.Linq.Expressions;

namespace SimPlanner.Tests
{
    public class LoggingVisitor : ExpressionVisitor
    {
        public LoggingVisitor()
        {
        }
        public override Expression Visit(Expression node)
        {
            if (node !=null){
                Console.WriteLine("Visit:");
                Console.WriteLine('\t' + node.GetType().ToString());
                Console.WriteLine('\t' + node.ToString());
                
            }
            return base.Visit(node);
        }
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.CatchBlock.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override CatchBlock VisitCatchBlock(CatchBlock node) {
            Console.WriteLine("VisitCatchBlock:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitCatchBlock(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.ElementInit.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override ElementInit VisitElementInit(ElementInit node) {
            Console.WriteLine("VisitElementInit:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitElementInit(node);}
        //
        // Summary:
        //     Visits the System.Linq.Expressions.LabelTarget.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override LabelTarget VisitLabelTarget(LabelTarget node) {
            Console.WriteLine("VisitLabelTarget:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitLabelTarget(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.MemberAssignment.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node) {
            Console.WriteLine("VisitMemberAssignment:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitMemberAssignment(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.MemberBinding.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override MemberBinding VisitMemberBinding(MemberBinding node) {
            Console.WriteLine("VisitMemberBinding:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitMemberBinding(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.MemberListBinding.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node) {
            Console.WriteLine("VisitMemberListBinding:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitMemberListBinding(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.MemberMemberBinding.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node) {
            Console.WriteLine("VisitMemberMemberBinding:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitMemberMemberBinding(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.SwitchCase.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override SwitchCase VisitSwitchCase(SwitchCase node) {
            Console.WriteLine("VisitSwitchCase:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitSwitchCase(node);}

        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.BinaryExpression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitBinary(BinaryExpression node)  {
            Console.WriteLine("VisitBinary:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitBinary(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.BlockExpression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitBlock(BlockExpression node)  {
            Console.WriteLine("VisitBlock:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitBlock(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.ConditionalExpression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitConditional(ConditionalExpression node)  {
            Console.WriteLine("VisitConditional:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitConditional(node);}
        //
        // Summary:
        //     Visits the System.Linq.Expressions.ConstantExpression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitConstant(ConstantExpression node)  {
            Console.WriteLine("VisitConstant:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitConstant(node);}
        //
        // Summary:
        //     Visits the System.Linq.Expressions.DebugInfoExpression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitDebugInfo(DebugInfoExpression node)  {
            Console.WriteLine("VisitDebugInfo:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitDebugInfo(node);}
        //
        // Summary:
        //     Visits the System.Linq.Expressions.DefaultExpression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitDefault(DefaultExpression node)  {
            Console.WriteLine("VisitDefault:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitDefault(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.DynamicExpression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitDynamic(DynamicExpression node)  {
            Console.WriteLine("VisitDynamic:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitDynamic(node);}
        //
        // Summary:
        //     Visits the children of the extension expression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitExtension(Expression node)  {
            Console.WriteLine("VisitExtension:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitExtension(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.GotoExpression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitGoto(GotoExpression node)  {
            Console.WriteLine("VisitGoto:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitGoto(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.IndexExpression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitIndex(IndexExpression node)  {
            Console.WriteLine("VisitIndex:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitIndex(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.InvocationExpression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitInvocation(InvocationExpression node)  {
            Console.WriteLine("VisitInvocation:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitInvocation(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.LabelExpression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitLabel(LabelExpression node)  {
            Console.WriteLine("VisitLabel:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitLabel(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.Expression`1.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Type parameters:
        //   T:
        //     The type of the delegate.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            Console.Write("VisitLambda<{0}>:",typeof(T).Name);
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitLambda(node);
        }
    //
    // Summary:
    //     Visits the children of the System.Linq.Expressions.ListInitExpression.
    //
    // Parameters:
    //   node:
    //     The expression to visit.
    //
    // Returns:
    //     The modified expression, if it or any subexpression was modified; otherwise,
    //     returns the original expression.
    protected override Expression VisitListInit(ListInitExpression node)  {
            Console.WriteLine("VisitListInit:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitListInit(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.LoopExpression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitLoop(LoopExpression node)  {
            Console.WriteLine("VisitLoop:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitLoop(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.MemberExpression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitMember(MemberExpression node)  {
            Console.WriteLine("VisitMember:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitMember(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.MemberInitExpression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitMemberInit(MemberInitExpression node)  {
            Console.WriteLine("VisitMemberInit:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitMemberInit(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.MethodCallExpression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitMethodCall(MethodCallExpression node)  {
            Console.WriteLine("VisitMethodCall:");
            Console.WriteLine(node.Method.ToString());
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            Console.WriteLine(node);
            return base.VisitMethodCall(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.NewExpression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitNew(NewExpression node)  {
            Console.WriteLine("VisitNew:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitNew(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.NewArrayExpression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitNewArray(NewArrayExpression node)  {
            Console.WriteLine("VisitNewArray:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitNewArray(node);}
        //
        // Summary:
        //     Visits the System.Linq.Expressions.ParameterExpression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitParameter(ParameterExpression node)  {
            Console.WriteLine("VisitParameter:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitParameter(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.RuntimeVariablesExpression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)  {
            Console.WriteLine("VisitRuntimeVariables:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitRuntimeVariables(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.SwitchExpression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitSwitch(SwitchExpression node)  {
            Console.WriteLine("VisitSwitch:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitSwitch(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.TryExpression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitTry(TryExpression node)  {
            Console.WriteLine("VisitTry:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitTry(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.TypeBinaryExpression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitTypeBinary(TypeBinaryExpression node)  {
            Console.WriteLine("VisitTypeBinary:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitTypeBinary(node);}
        //
        // Summary:
        //     Visits the children of the System.Linq.Expressions.UnaryExpression.
        //
        // Parameters:
        //   node:
        //     The expression to visit.
        //
        // Returns:
        //     The modified expression, if it or any subexpression was modified; otherwise,
        //     returns the original expression.
        protected override Expression VisitUnary(UnaryExpression node)  {
            Console.WriteLine("VisitUnary:");
            Console.WriteLine('\t' + node.GetType().ToString());
            Console.WriteLine('\t' + node.ToString());
            return base.VisitUnary(node);}

    }
}
