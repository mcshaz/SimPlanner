using SP.Dto.ProcessBreezeRequests;
using SP.Dto.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SP.Dto.Maps
{
    public static class MapperConfig
    {
        private static readonly IReadOnlyDictionary<Type, IDomainDtoMap> _dtoMapDictionary;
        private static readonly IReadOnlyDictionary<Type, IDomainDtoMap> _serverDtoDictionary;
        static MapperConfig()
        {
            var maps = new IDomainDtoMap[]
            {
                new ActivityMaps(),
                new CandidatePrereadingMaps(),
                new CultureMaps(),
                new CourseMaps(),
                new CourseActivityMaps(),
                new CourseDayMaps(),
                new CourseFormatMaps(),
                new CourseParticipantMaps(),
                new CourseScenarioFacultyRoleMaps(),
                new CourseSlotMaps(),
                new CourseSlotManikinMaps(),
                new CourseSlotPresenterMaps(),
                new CourseSlotScenarioMaps(),
                new CourseTypeMaps(),
                new CourseTypeDepartmentMaps(),
                new CourseTypeScenarioRoleMaps(),
                new DepartmentMaps(),
                new FacultyScenarioRoleMaps(),
                new HotDrinkMap(),
                new InstitutionMaps(),
                new ManikinMaps(),
                new ManikinModelMaps(),
                new ManikinManufacturerMaps(),
                new ManikinServiceMaps(),
                new ParticipantMaps(),
                new ProfessionalRoleMaps(),
                new ProfessionalRoleInstitutionMaps(),
                new RoomMaps(),
                new ScenarioMaps(),
                new ScenarioResourceMaps(),
                new UserRoleMaps()
            };
            _dtoMapDictionary = new ReadOnlyDictionary<Type, IDomainDtoMap>(
                maps.ToDictionary(kv=>kv.TypeofDto));
            _serverDtoDictionary = new ReadOnlyDictionary<Type, IDomainDtoMap>(
                maps.ToDictionary(kv => kv.TypeofServerObject));
        }

        public static Type GetServerModelType(Type dtoType)
        {
            return _dtoMapDictionary[dtoType].TypeofServerObject;
        }

        public static Func<object, object> GetFromDtoMapper(Type dtoType)
        {
            return _dtoMapDictionary[dtoType].MapFromDto;
        }

        public static LambdaExpression GetWhereExpression(Type serverModelType, CurrentPrincipal user)
        {
            return _serverDtoDictionary[serverModelType].GetWhereExpression(user);
        }

        public static Type GetDtoType(Type serverModelType)
        {
            return _serverDtoDictionary[serverModelType].TypeofDto;
        }

        public static object MapFromDto(object obj)
        {
            return MapFromDto(obj.GetType(), obj);
        }

        public static object MapFromDto(Type dtoType, object obj)
        {
            return _dtoMapDictionary[dtoType].MapFromDto(obj);
        }
        const char defaultSepChar = '.';
        public static IQueryable<TMap> ProjectToDto<T, TMap>(this IQueryable<T> queryable, CurrentPrincipal currentUser, string[] includes = null, string[] selects=null, char sepChar= defaultSepChar)
        {
            var returnVar = GetToDtoLambda(typeof(TMap), currentUser, includes, selects, sepChar);
            if (returnVar.WhereExpression != null)
            {
                queryable = queryable.Where((Expression<Func<T, bool>>)returnVar.WhereExpression);
            }
            return queryable.Select((Expression<Func<T, TMap>>)returnVar.SelectExpression);
        }

        /// <summary>
        /// predominantly for unit testing
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TMap"></typeparam>
        /// <param name="currentUser"></param>
        /// <param name="includes"></param>
        /// <param name="selects"></param>
        /// <param name="sepChar"></param>
        /// <returns></returns>
        public static Expression<Func<T,TMap>> GetToDtoLambda<T,TMap>(CurrentPrincipal currentUser, string[] includes = null, string[] selects = null, char sepChar = defaultSepChar)
        {
            var returnVar = GetToDtoLambda(typeof(TMap), currentUser, includes, selects, sepChar);
            return (Expression<Func<T, TMap>>)returnVar.SelectExpression;
        }

        internal static Node GetToDtoLambda(Type dtoType, CurrentPrincipal currentUser, string[] includes = null, string[] selects = null, char sepChar = defaultSepChar)
        {
            var includeSelects = new IncludeSelectOptions(dtoType, currentUser,includes, selects, sepChar);
            VisitNodes(includeSelects.RequiredMappings);
            return includeSelects.RequiredMappings;
        }

        private static void VisitNodes(Node includeTree)
        {
            if (includeTree == null) { throw new ArgumentNullException("includeTree"); }
            
            if (includeTree.Children.Any())
            {
                foreach(var n in includeTree.Children)
                {
                    VisitNodes(n);
                }
                includeTree.SelectExpression = includeTree.SelectExpression.MapNavProperty(includeTree.Children.Select(c => new NavProperty(c.PropertyInfo, c.SelectExpression) { Where = c.WhereExpression }));  
            }
        }

        private class IncludeSelectOptions
        {
            readonly char _sepChar;
            readonly CurrentPrincipal _currentUser;
            internal readonly Node RequiredMappings;

            public IncludeSelectOptions(Type dtoType, CurrentPrincipal currentUser,IList<string> includes = null, IList<string> selects = null, char sepChar = '.')
            {
                ValidateNoRepeats(includes, "includes");
                ValidateNoRepeats(selects, "selects");
                _sepChar = sepChar;
                _currentUser = currentUser;
                List<string[]> includeList = (includes == null) ? new List<string[]>() : new List<string[]>(includes.Select(i => i.Split(sepChar)));
                List<string[]> selectList = (selects == null) ? new List<string[]>() : new List<string[]>(selects.Select(i => i.Split(sepChar)));

                RequiredMappings = GetRequiredMappings(dtoType, includeList, selectList);

                //RequiredMappings.PrintPretty("  ");
            }

            [Conditional("DEBUG")]
            private static void ValidateNoRepeats(IEnumerable<string> includes, string paramName)
            {
                if (includes == null) { return;  }
                var repeats = includes.Repeats();
                if (repeats.Any())
                {
                    throw new ArgumentException("property [" + string.Join(",", repeats) + "] is repeated", paramName);
                }
            }
            private Node GetRequiredMappings(Type dtoType, IList<string[]> includeList, IList<string[]> selectList)
            {
                var dict = _dtoMapDictionary[dtoType];
                Node node = new Node(dtoType) {
                    SelectExpression = dict.MapToDto,
                    WhereExpression = dict.GetWhereExpression(_currentUser)
                };
                GetRequiredMappings(node, includeList, true);
                GetRequiredMappings(node, selectList, false); 
                return node;
            }
            private Node GetRequiredMappings(Node node, IList<string[]> mapList, bool throwIfLastNotFound)
            {
                //build tree
                for(int i=0;i< mapList.Count;i++)
                {
                    Node match = node;
                    int j;
                    for (j=0; j < mapList[i].Length; j++)
                    {
                        string propertyName = mapList[i][j];
                        var matchingChild = match.Children.FirstOrDefault(n => n.PropertyInfo.Name == propertyName);
                        if (matchingChild == null)
                        {
                            PropertyInfo includeInfo = match.Type.GetProperty(propertyName);
                            if (includeInfo == null)
                            {
                                throw new ArgumentException($"could not find property {propertyName} on type {match.Type.FullName}");
                            }
                            Type baseType = includeInfo.PropertyType.IsGenericType && includeInfo.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
                                ?includeInfo.PropertyType.GenericTypeArguments[0]
                                :includeInfo.PropertyType;
                            IDomainDtoMap map;
                            if (_dtoMapDictionary.TryGetValue(baseType, out map))
                            {
                                matchingChild = new Node(baseType) {
                                    SelectExpression = map.MapToDto,
                                    WhereExpression = map.GetWhereExpression(_currentUser),
                                    PropertyInfo = includeInfo
                                };
                                match.Children.Add(matchingChild);
                            }
                            else if (throwIfLastNotFound || j+1<mapList[i].Length)
                            {
                                throw new KeyNotFoundException($"Could not find map for property {propertyName} [{baseType.Name}]");
                            }
                        }
                        match = matchingChild;
                    }
                }
                return node;
            }
        }

        internal class Node
        {
            public readonly List<Node> Children;
            public Type Type { get; set; }
            public PropertyInfo PropertyInfo { get; set; }
            public LambdaExpression SelectExpression { get; set; }
            public LambdaExpression WhereExpression { get; set; }
            public Node(Type type)
            {
                Type = type;
                Children = new List<Node>();
            }
            public void PrintPretty(string indent, bool last = false)
            {
                Debug.Write(indent);
                if (last)
                {
                    Debug.Write("\\-");
                    indent += "  ";
                }
                else
                {
                    Debug.Write("|-");
                    indent += "| ";
                }
                Debug.WriteLine(Type.Name);

                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].PrintPretty(indent, i == Children.Count - 1);
                }
            }
        }
    }

}
