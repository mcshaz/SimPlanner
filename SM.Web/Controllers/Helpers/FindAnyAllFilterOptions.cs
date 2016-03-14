using Microsoft.Data.OData.Query;
using Microsoft.Data.OData.Query.SemanticAst;
using System;
using System.Collections.Generic;
using System.Web.Http.OData.Query;

namespace SM.Web.Controllers.Helpers
{
    public class FindAnyAllFilterOptions
    {
        public static IList<string> GetPaths(FilterQueryOption filterOption)
        {
            var findAnyAll = new FindAnyAllFilterOptions();
            findAnyAll.Find(filterOption.FilterClause.Expression);
            return findAnyAll.Paths;
        }
        public readonly IList<string> Paths = new List<string>();
        public void Find(QueryNode node)
        {
            switch (node.Kind)
            {
                case QueryNodeKind.All:
                case QueryNodeKind.Any:
                    var l = (LambdaNode)node;
                    Paths.Add(((CollectionNavigationNode)l.Source).NavigationProperty.Name);
                    Find(l.Body);
                    break;
                case QueryNodeKind.BinaryOperator:
                    var bo = (BinaryOperatorNode)node;
                    Find(bo.Left);
                    Find(bo.Right);
                    break;
                case QueryNodeKind.NonentityRangeVariableReference:

                case QueryNodeKind.UnaryOperator:
                    var uo = (UnaryOperatorNode)node;
                    Find(uo.Operand);
                    break;

                case QueryNodeKind.SingleValuePropertyAccess:
                    var sv = (SingleValuePropertyAccessNode)node;
                    Find(sv.Source);
                    break;
                /*
        case QueryNodeKind.CollectionPropertyAccess:
            var cpa = (CollectionPropertyAccessNode)node;
            Paths[Paths.Count-1] += '.' + cpa.Property.Name;
            break;    
                                    */
                case QueryNodeKind.CollectionNavigationNode:
                    var cnn = (CollectionNavigationNode)node;
                    Paths[Paths.Count - 1] += '.' + cnn.NavigationProperty.Name;
                    break;
                case QueryNodeKind.SingleNavigationNode:
                    var snn = (SingleNavigationNode)node;
                    Paths[Paths.Count - 1] += '.' + snn.NavigationProperty.Name;
                    break;
                //case QueryNodeKind.SingleValueOpenPropertyAccess:        
                //case QueryNodeKind.SingleEntityCast:        

                //case QueryNodeKind.EntityCollectionCast:             
                case QueryNodeKind.NamedFunctionParameter:
                    Console.WriteLine(node.GetType());
                    break;
            }
        }
    }
}
