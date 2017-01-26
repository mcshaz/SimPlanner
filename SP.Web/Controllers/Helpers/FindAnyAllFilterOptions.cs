using Microsoft.Data.OData.Query;
using Microsoft.Data.OData.Query.SemanticAst;
using SP.Dto.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.OData.Query;

namespace SP.Web.Controllers.Helpers
{
    public static class FindNavigationFilterOptions
    {
        public static IEnumerable<string> GetPaths(FilterQueryOption filterOption, string separator = ".")
        {
            Console.WriteLine("------");
            var v = new MyVisitor(separator);
            IEnumerable<string> returnVar;
            var expr = filterOption.FilterClause.Expression;
            var t = expr.GetType();
            if (t == typeof(BinaryOperatorNode))
            {
                returnVar = v.Visit((BinaryOperatorNode)expr);
            }
            else if (t == typeof(AnyNode))
            {
                returnVar = v.Visit((AnyNode)expr);
            }
            else
            {
                throw new TypeAccessException(t.FullName);
            }
            return returnVar.ToHashSet();
        }

        /*
        public static List<string> GetPaths(string separator = ".")
        {
            return (from p in _paths
                    where p.Count > 0
                    select string.Join(separator, p)).ToList();
        }
        */
        private class MyVisitor : QueryNodeVisitor<IEnumerable<string>>
        {
            public MyVisitor(string seperator=".") : base()
            {
                BaseString = string.Empty;
                _seperator = seperator;
            }
            private string _seperator;
            private string BaseString { get; set; }
            private readonly static string[] _emptyString = new string[0];

            private string Combine(QueryNode source, string name = null)
            {
                var sourceString = source.Accept(this).FirstOrDefault();
                if (!string.IsNullOrEmpty(sourceString) && BaseString.Length > 0 && sourceString.StartsWith(BaseString))
                {
                    sourceString = sourceString.Substring(BaseString.Length + 1);
                }
                var stringList = (new[] { BaseString, sourceString, name}).Where(s => !string.IsNullOrEmpty(s));
                return string.Join(_seperator, stringList);
            }
            public override IEnumerable<string> Visit(CollectionPropertyAccessNode nodeIn)
            {
                return nodeIn.Source.Accept(this);
            }
            public override IEnumerable<string> Visit(SingleNavigationNode nodeIn)
            {
                Console.WriteLine("SNN");
                return new[] {
                    Combine(nodeIn.Source,nodeIn.NavigationProperty.Name)
                };
            }
            public override IEnumerable<string> Visit(CollectionNavigationNode nodeIn)
            {
                Console.WriteLine("CNN");
                return new[] 
                {
                    Combine(nodeIn.Source,nodeIn.NavigationProperty.Name)
                };
            }
            public override IEnumerable<string> Visit(SingleValuePropertyAccessNode nodeIn)
            {
                return nodeIn.Source.Accept(this);//nodeIn.GetType().Name + ' ' + nodeIn.Property.Name + Environment.NewLine;
            }
            public override IEnumerable<string> Visit(BinaryOperatorNode nodeIn)
            {
                return nodeIn.Left.Accept(this)
                    .Concat(nodeIn.Right.Accept(this));
            }
            private IEnumerable<string> VisitLambda(LambdaNode nodeIn)
            {
                var oldBase = BaseString;
                BaseString = Combine(nodeIn.Source);
                var returnVar = nodeIn.Body.Accept(this);
                if (!returnVar.Any() && BaseString != oldBase)
                {
                    returnVar = new[] { BaseString };
                }
                BaseString = oldBase;
                return returnVar;
            }
            public override IEnumerable<string> Visit(AllNode nodeIn)
            {
                return VisitLambda(nodeIn);
            }
            public override IEnumerable<string> Visit(AnyNode nodeIn)
            {
                return VisitLambda(nodeIn);
            }
            public override IEnumerable<string> Visit(CollectionFunctionCallNode nodeIn)
            {
                return nodeIn.Source.Accept(this);
            }
            public override IEnumerable<string> Visit(ConstantNode nodeIn)
            {
                return _emptyString;
            }
            public override IEnumerable<string> Visit(ConvertNode nodeIn)
            {
                return nodeIn.Source.Accept(this);
            }
            public override IEnumerable<string> Visit(EntityCollectionCastNode nodeIn)
            {
                return nodeIn.Source.Accept(this);
            }
            public override IEnumerable<string> Visit(EntityCollectionFunctionCallNode nodeIn)
            {
                return nodeIn.Source.Accept(this);
            }
            public override IEnumerable<string> Visit(EntityRangeVariableReferenceNode nodeIn)
            {
                return _emptyString;
            }
            public override IEnumerable<string> Visit(NamedFunctionParameterNode nodeIn)
            {
                return _emptyString;
            }
            public override IEnumerable<string> Visit(NonentityRangeVariableReferenceNode nodeIn)
            {
                return _emptyString;
            }
            public override IEnumerable<string> Visit(SingleValueFunctionCallNode nodeIn)
            {
                return _emptyString;
            }
            public override IEnumerable<string> Visit(SingleEntityCastNode nodeIn)
            {
                return nodeIn.Source.Accept(this);
            }
            public override IEnumerable<string> Visit(SingleEntityFunctionCallNode nodeIn)
            {
                return nodeIn.Source.Accept(this);
            }

            public override IEnumerable<string> Visit(SingleValueOpenPropertyAccessNode nodeIn)
            {
                return nodeIn.Source.Accept(this);
            }

            public override IEnumerable<string> Visit(UnaryOperatorNode nodeIn)
            {
                return nodeIn.Operand.Accept(this);
            }
        }
    }
}
