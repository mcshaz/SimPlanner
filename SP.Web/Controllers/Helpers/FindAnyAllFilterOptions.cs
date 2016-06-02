using Microsoft.Data.OData.Query;
using Microsoft.Data.OData.Query.SemanticAst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.OData.Query;

namespace SP.Web.Controllers.Helpers
{
    public class FindAnyAllFilterOptions
    {
        public FindAnyAllFilterOptions()
        {
            _paths = new List<IList<string>>();
            addPath();
        }
        public static IList<string> GetPaths(FilterQueryOption filterOption, string separator = ".")
        {
            var findAnyAll = new FindAnyAllFilterOptions();
            findAnyAll.Find(filterOption.FilterClause.Expression);

            return findAnyAll.GetPaths(separator);
        }
        readonly IList<IList<string>> _paths;
        void addPath()
        {
            _paths.Add(new List<string>());
        }
        void AddNav(string navName)
        {
            _paths[_paths.Count - 1].Add(navName);
        }
        public List<string> GetPaths(string separator = ".")
        {
            return (from p in _paths
                    where p.Count > 0
                    select string.Join(separator, p)).ToList();
        }
        public void Find(QueryNode node)
        {
            switch (node.Kind)
            {
                case QueryNodeKind.All:
                case QueryNodeKind.Any:
                    var l = (LambdaNode)node;
                    Find(l.Source);
                    Find(l.Body);
                    break;
                case QueryNodeKind.BinaryOperator:
                    var bo = (BinaryOperatorNode)node;
                    Find(bo.Left);
                    addPath();
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
                    Find(cnn.Source);
                    AddNav(cnn.NavigationProperty.Name);
                    break;
                case QueryNodeKind.SingleNavigationNode:
                    var snn = (SingleNavigationNode)node;
                    AddNav(snn.NavigationProperty.Name);
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
