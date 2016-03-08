using SM.DataAccess;
using SM.DTOs.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace SM.DTOs.Maps
{
    public static class MapperConfig
    {
        private static IReadOnlyDictionary<string, LambdaExpression> _maps;
        static MapperConfig()
        {
            _maps = new ReadOnlyDictionary<string, LambdaExpression>(
                    CreateMapDictionary(new[] {
                        new DtoMap(typeof(Country),CountryMaps.mapFromRepo(), "Countries"),
                        new DtoMap(typeof(CountryLocaleCode),  CountryLocaleCodeMaps.mapFromRepo()),
                        new DtoMap(typeof(Department),  DepartmentMaps.mapFromRepo()),
                        new DtoMap(typeof(ScenarioRoleDescription),  ScenarioRoleDescriptionMaps.mapFromRepo()),
                        new DtoMap(typeof(Institution),  InstitutionMaps.mapFromRepo()),
                        new DtoMap(typeof(Manequin),  ManequinMaps.mapFromRepo()),
                        new DtoMap(typeof(ManequinManufacturer),  ManequinManufacturerMaps.mapFromRepo()),
                        new DtoMap(typeof(Participant),  ParticipantMaps.mapFromRepo()),
                        new DtoMap(typeof(ProfessionalRole),  ProfessionalRoleMaps.mapFromRepo()),
                        new DtoMap(typeof(Scenario),  ScenarioMaps.mapFromRepo()),
                        new DtoMap(typeof(ScenarioResource),  ScenarioResourceMaps.mapFromRepo()),
                        new DtoMap(typeof(Course),  CourseMaps.mapFromRepo()),
                        new DtoMap(typeof(CourseParticipant),  CourseParticipantMaps.mapFromRepo()),
                        new DtoMap(typeof(CourseType),  CourseTypeMaps.mapFromRepo()),
                        new DtoMap(typeof(CourseSlot),  CourseSlotMaps.mapFromRepo()),
                        new DtoMap(typeof(ScenarioSlot),  ScenarioSlotMaps.mapFromRepo()),
                        new DtoMap(typeof(CourseTeachingResource),  CourseTeachingResourceMaps.mapFromRepo()),
                        new DtoMap(typeof(CourseSlotPresenter),  CourseSlotPresenterMaps.mapFromRepo()),
                        new DtoMap(typeof(ScenarioFacultyRole),  ScenarioFacultyRoleMaps.mapFromRepo()),
                        new DtoMap(typeof(Room),  RoomMaps.mapFromRepo())
                    }).ToDictionary(kv=>kv.Key, kv=>kv.Value)
                );
        }

        static IEnumerable<KeyValuePair<string,LambdaExpression>> CreateMapDictionary(IEnumerable<DtoMap> maps)
        {
            foreach (var m in maps)
            {
                yield return new KeyValuePair<string, LambdaExpression>(m.TypeName, m.Map);
                yield return new KeyValuePair<string, LambdaExpression>(m.PleuralName, m.Map);
            }
        }
        public static IQueryable<TMap> Project<T, TMap>(this IQueryable<T> queryable, IncludeSelectOptions includeSelects = null)
        {
            return queryable.Select(GetLambda<T, TMap>(includeSelects));
        }

        public static Expression<Func<T,TMap>> GetLambda<T,TMap>(IncludeSelectOptions includeSelects = null)
        {
            return (Expression<Func<T, TMap>>)GetLambda(typeof(T).Name, includeSelects);
        }

        public static LambdaExpression GetLambda(string typeName, IncludeSelectOptions includeSelects = null)
        {
            var returnVar = _maps[typeName];
            if (includeSelects != null && includeSelects.RequiredMappings.Any())
            {
                var navs = includeSelects.RequiredMappings.Select(i=>MapChildren(i));
                return returnVar.MapNavProperty(navs);
            }
            return returnVar;
        }

        private class DtoMap
        {
            public DtoMap(Type t, LambdaExpression map, string pleuralName=null)
            {
                TypeName = t.Name;
                PleuralName = pleuralName ?? t.Name + 's';
                Map = map;
            }
            public readonly string TypeName;
            public readonly string PleuralName;
            public readonly LambdaExpression Map;
        }

        public static KeyValuePair<string, LambdaExpression> MapChildren(IList<string> includes)
        {
            if (includes.Count == 0) { throw new ArgumentException("must contain at least 1 element", "includes"); }
            int lastIndex = includes.Count - 1;
            LambdaExpression returnLambda = _maps[includes[lastIndex]];

            for (int i = --lastIndex; i >= 0; i--)
            {
                returnLambda = _maps[includes[i]].MapNavProperty(new[] { new KeyValuePair<string, LambdaExpression>(includes[i + 1], returnLambda) });
            }

            return new KeyValuePair<string, LambdaExpression>(includes[0], returnLambda);
        }

        public class IncludeSelectOptions
        {
            readonly char _sepChar;
            public readonly ReadOnlyCollection<string[]> RequiredMappings;

            public IncludeSelectOptions(IList<string> includes = null, IList<string> selects = null, char sepChar = '.')
            {
                ValidateNoRepeats(includes, "includes");
                ValidateNoRepeats(selects, "selects");
                _sepChar = sepChar;
                List<string[]> mappings = (includes == null) ? new List<string[]>() : new List<string[]>(includes.Select(i => i.Split(sepChar)));

                if (selects != null)
                {
                    mappings.AddRange(SelectNavProperties(selects.Select(s => s.Split(sepChar))));
                }

                RequiredMappings = GetRequiredIncludes(mappings).AsReadOnly();
            }

            static IEnumerable<string[]> SelectNavProperties(IEnumerable<string[]> selects)
            {
                foreach (var s in selects)
                {
                    if (_maps.ContainsKey(s[s.Length - 1]))
                    {
                        yield return s;
                    }
                    if (s.Length > 1)
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

            static List<string[]> GetRequiredIncludes(IList<string[]> includeList)
            {
                var returnVar = new string[includeList.Count][];

                includeList.CopyTo(returnVar, 0);
                int last = returnVar.Length - 1;
                for (int i = 0; i < last; i++)
                {
                    for (int j = i+1; j < returnVar.Length; j++)
                    {
                        if (returnVar[j].StartsWith(returnVar[i]))
                        {
                            returnVar[i] = null;
                        }
                        else if (returnVar[i].StartsWith(returnVar[j]))
                        {
                            returnVar[j] = returnVar[i];
                            returnVar[i] = null;
                        }
                    }
                }

                return returnVar.Where(r => r != null).ToList();
            }

        }
    }

}
