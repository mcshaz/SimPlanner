using SP.DataAccess;
using SP.Dto.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace SP.Dto.Maps
{
    public static class MapperConfig
    {
        private static IReadOnlyDictionary<string, LambdaExpression> _maps;
        static MapperConfig()
        {
            _maps = new ReadOnlyDictionary<string, LambdaExpression>(
                CreateMapDictionary(
                    new[] {
                        new DtoMap(typeof(ActivityTeachingResource),  ActivityTeachingResourceMaps.MapFromDomain(),false,"ActivityChoices"),
                        new DtoMap(typeof(ChosenTeachingResource),ChosenTeachingResourceMaps.MapFromDomain()),
                        new DtoMap(typeof(Culture),CultureMaps.MapFromDomain()),
                        new DtoMap(typeof(Course),  CourseMaps.MapFromDomain(), true, "OutreachCourses"),
                        new DtoMap(typeof(CourseActivity),  CourseActivityMaps.MapFromDomain(), false, "CourseActivities", "Activity"),
                        new DtoMap(typeof(CourseDay),  CourseDayMaps.MapFromDomain()),
                        new DtoMap(typeof(CourseFormat),  CourseFormatMaps.MapFromDomain()),
                        new DtoMap(typeof(CourseParticipant),  CourseParticipantMaps.MapFromDomain()),
                        new DtoMap(typeof(CourseScenarioFacultyRole),  CourseScenarioFacultyRoleMaps.MapFromDomain()),
                        new DtoMap(typeof(CourseSlot),  CourseSlotMaps.MapFromDomain()),
                        new DtoMap(typeof(CourseSlotManequin),  CourseSlotManequinMaps.MapFromDomain()),
                        new DtoMap(typeof(CourseSlotPresenter),  CourseSlotPresenterMaps.MapFromDomain()),
                        new DtoMap(typeof(CourseSlotScenario),  CourseSlotScenarioMaps.MapFromDomain()),
                        new DtoMap(typeof(CourseType),  CourseTypeMaps.MapFromDomain()),
                        new DtoMap(typeof(CourseTypeDepartment),  CourseTypeDepartmentMaps.MapFromDomain()),
                        new DtoMap(typeof(CourseTypeScenarioRole),  CourseTypeScenarioRoleMaps.MapFromDomain()),
                        new DtoMap(typeof(Department),  DepartmentMaps.MapFromDomain(), true,"OutreachingDepartment"),
                        new DtoMap(typeof(FacultyScenarioRole),  FacultyScenarioRoleMaps.MapFromDomain()),
                        new DtoMap(typeof(Institution),  InstitutionMaps.MapFromDomain()),
                        new DtoMap(typeof(Manequin),  ManequinMaps.MapFromDomain()),
                        new DtoMap(typeof(ManequinModel),  ManequinModelMaps.MapFromDomain(),true,"Model", "Models"),
                        new DtoMap(typeof(ManequinManufacturer),  ManequinManufacturerMaps.MapFromDomain()),
                        new DtoMap(typeof(ManequinService),  ManequinServiceMaps.MapFromDomain()),
                        new DtoMap(typeof(Participant),  ParticipantMaps.MapFromDomain()),
                        new DtoMap(typeof(ProfessionalRole),  ProfessionalRoleMaps.MapFromDomain()),
                        new DtoMap(typeof(ProfessionalRoleInstitution),  ProfessionalRoleInstitutionMaps.MapFromDomain()),
                        new DtoMap(typeof(Room),  RoomMaps.MapFromDomain()),
                        new DtoMap(typeof(Scenario),  ScenarioMaps.MapFromDomain()),
                        new DtoMap(typeof(ScenarioResource),  ScenarioResourceMaps.MapFromDomain()),
                    }).ToDictionary(kv=>kv.Key, kv=>kv.Value));
        }

        static IEnumerable<KeyValuePair<string, LambdaExpression>> CreateMapDictionary(IEnumerable<DtoMap> maps)
        {
            foreach (var m in maps)
            {
                foreach (var t in m.PropertyNames)
                {
                    yield return new KeyValuePair<string, LambdaExpression>(t, m.Map);
                }
            }
        }
        public static IQueryable<TMap> Project<T, TMap>(this IQueryable<T> queryable, string[] includes = null, string[] selects=null, char sepChar='.')
        {
            return queryable.Select(GetLambda<T, TMap>(includes, selects, sepChar));
        }

        public static Expression<Func<T,TMap>> GetLambda<T,TMap>(string[] includes = null, string[] selects = null, char sepChar = '.')
        {
            return (Expression<Func<T, TMap>>)GetLambda(typeof(T).Name, includes, selects, sepChar);
        }

        internal static LambdaExpression GetLambda(string typeName, string[] includes, string[] selects, char sepChar)
        {
            var includeSelects = new IncludeSelectOptions(typeName, includes, selects, sepChar);
            VisitNodes(includeSelects.RequiredMappings);
            return includeSelects.RequiredMappings.Lambda;
        }

        private class DtoMap
        {
            public DtoMap(Type t, LambdaExpression map, bool simple_s_pleuralisation = true, params string[] aliases)
            {
                PropertyNames = aliases.Concat(new[] { t.Name });
                if (simple_s_pleuralisation) { PropertyNames = PropertyNames.Concat(new[] { t.Name + 's' }); }
                Map = map;
            }
            public readonly IEnumerable<string> PropertyNames;
            public readonly LambdaExpression Map;
        }

        private static void VisitNodes(IncludeSelectOptions.Node includeTree)
        {
            if (includeTree == null) { throw new ArgumentNullException("includes"); }
            
            if (includeTree.Children.Any())
            {
                foreach(var n in includeTree.Children)
                {
                    VisitNodes(n);
                }
                includeTree.Lambda = _maps[includeTree.Name].MapNavProperty(includeTree.Children.Select(c => new KeyValuePair<string, LambdaExpression>(c.Name, c.Lambda)));  
            }
            else
            {
                includeTree.Lambda = _maps[includeTree.Name];
            }
        }

        private class IncludeSelectOptions
        {
            readonly char _sepChar;
            internal readonly Node RequiredMappings;

            public IncludeSelectOptions(string typeName, IList<string> includes = null, IList<string> selects = null, char sepChar = '.')
            {
                ValidateNoRepeats(includes, "includes");
                ValidateNoRepeats(selects, "selects");
                _sepChar = sepChar;
                List<string[]> mappings = (includes == null) ? new List<string[]>() : new List<string[]>(includes.Select(i => i.Split(sepChar)));

                if (selects != null)
                {
                    mappings.AddRange(SelectNavProperties(selects.Select(s => s.Split(sepChar))));
                }

                RequiredMappings = GetRequiredIncludes(typeName, mappings);

                //RequiredMappings.PrintPretty("  ");
            }

            static IEnumerable<string[]> SelectNavProperties(IEnumerable<string[]> selects)
            {
                foreach (var s in selects)
                {
                    if (_maps.ContainsKey(s[s.Length - 1]))
                    {
                        yield return s;
                    }
                    else
                    {
                        var returnVar = new string[s.Length - 1];
                        Array.Copy(s, returnVar, returnVar.Length);
                        yield return returnVar;
                    }
                }
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

            static Node GetRequiredIncludes(string typeName, IList<string[]> includeList)
            {
                //build tree
                Node returnVar = new Node(typeName);
                for(int i=0;i< includeList.Count;i++)
                {
                    Node match = returnVar;
                    int j;
                    for (j=0; j < includeList[i].Length; j++)
                    {
                        var matchingChild = match.Children.FirstOrDefault(n => n.Name == includeList[i][j]);
                        if (matchingChild == null)
                        {
                            break;
                        }
                        match = matchingChild;
                    }
                    for (; j < includeList[i].Length; j++)
                    {
                        match.Children.Add(new Node(includeList[i][j]));
                        match = match.Children[match.Children.Count-1];
                    }
                }
                return returnVar;
            }

            internal class Node
            {
                public readonly List<Node> Children;
                public readonly string Name;
                public LambdaExpression Lambda { get; set; }
                public Node(string name)
                {
                    Name = name;
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
                    Debug.WriteLine(Name);

                    for (int i = 0; i < Children.Count; i++)
                    {
                        Children[i].PrintPretty(indent, i == Children.Count - 1);
                    }
                }
            }
        }
    }

}
