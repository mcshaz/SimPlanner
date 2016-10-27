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
        private static readonly IReadOnlyDictionary<Type, Type> _serverDtoDictionary;
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
            _serverDtoDictionary = new ReadOnlyDictionary<Type, Type>(
                maps.ToDictionary(kv => kv.TypeofServerObject, kv => kv.TypeofDto));
        }

        public static Type GetServerModelType(Type dtoType)
        {
            return _dtoMapDictionary[dtoType].TypeofServerObject;
        }

        public static Func<object, object> GetFromDtoMapper(Type dtoType)
        {
            return _dtoMapDictionary[dtoType].MapFromDto;
        }

        public static Type GetDtoType(Type serverModelType)
        {
            return _serverDtoDictionary[serverModelType];
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
        public static IQueryable<TMap> ProjectToDto<T, TMap>(this IQueryable<T> queryable, string[] includes = null, string[] selects=null, char sepChar= defaultSepChar)
        {
            return queryable.Select(GetToDtoLambda<T, TMap>(includes, selects, sepChar));
        }

        public static Expression<Func<T,TMap>> GetToDtoLambda<T,TMap>(string[] includes = null, string[] selects = null, char sepChar = defaultSepChar)
        {
            return (Expression<Func<T, TMap>>)GetToDtoLambda(typeof(TMap), includes, selects, sepChar);
        }

        public static LambdaExpression GetToDtoLambda(Type dtoType, string[] includes = null, string[] selects = null, char sepChar = defaultSepChar)
        {
            var includeSelects = new IncludeSelectOptions(dtoType, includes, selects, sepChar);
            VisitNodes(includeSelects.RequiredMappings);
            return includeSelects.RequiredMappings.Lambda;
        }

        private static void VisitNodes(IncludeSelectOptions.Node includeTree)
        {
            if (includeTree == null) { throw new ArgumentNullException("includeTree"); }
            
            if (includeTree.Children.Any())
            {
                foreach(var n in includeTree.Children)
                {
                    VisitNodes(n);
                }
                includeTree.Lambda = includeTree.Lambda.MapNavProperty(includeTree.Children.Select(c => new KeyValuePair<PropertyInfo, LambdaExpression>(c.PropertyInfo, c.Lambda)));  
            }
        }

        private class IncludeSelectOptions
        {
            readonly char _sepChar;
            internal readonly Node RequiredMappings;

            public IncludeSelectOptions(Type dtoType, IList<string> includes = null, IList<string> selects = null, char sepChar = '.')
            {
                ValidateNoRepeats(includes, "includes");
                ValidateNoRepeats(selects, "selects");
                _sepChar = sepChar;
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
            private static Node GetRequiredMappings(Type dtoType, IList<string[]> includeList, IList<string[]> selectList )
            {
                Node node = new Node(dtoType) { Lambda = _dtoMapDictionary[dtoType].MapToDto };
                GetRequiredMappings(node, includeList, true);
                GetRequiredMappings(node, selectList, false); 
                return node;
            }
            private static Node GetRequiredMappings(Node node, IList<string[]> mapList, bool throwIfLastNotFound)
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
                            Type baseType = includeInfo.PropertyType.IsGenericType && includeInfo.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
                                ?includeInfo.PropertyType.GenericTypeArguments[0]
                                :includeInfo.PropertyType;
                            IDomainDtoMap map;
                            if (_dtoMapDictionary.TryGetValue(baseType, out map))
                            {
                                matchingChild = new Node(baseType) {
                                    Lambda = map.MapToDto,
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

            internal class Node
            {
                public readonly List<Node> Children;
                public Type Type { get; set; }
                public PropertyInfo PropertyInfo { get; set;}
                public LambdaExpression Lambda { get; set; }
                public Node(Type type)
                {
                    Type = type;
                    Children = new List<Node>();
                }
                public void PrintPretty(string indent, bool last=false)
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

}
