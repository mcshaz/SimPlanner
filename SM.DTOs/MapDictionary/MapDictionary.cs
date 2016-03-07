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
    internal static class MapperConfig
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
        public static IQueryable<TMap> Project<T, TMap>(this IQueryable<T> queryable, params string[] includes)
        {
            return queryable.Select(GetLambda<T, TMap>(includes));
        }

        public static Expression<Func<T,TMap>> GetLambda<T,TMap>(params string[] includes)
        {
            return (Expression<Func<T, TMap>>)GetLambda(typeof(T).Name, includes);
        }

        public static LambdaExpression GetLambda(string typeName, params string[] includes)
        {
            var returnVar = _maps[typeName];
            if (includes.Any())
            {
                validateIncludes(returnVar, includes);
                var navs = GetMaps(includes).ToList();
                if (navs.Any())
                {
                    return returnVar.MapNavProperty(navs.Select(n=>n.MapChildren()));
                }
            }
            return returnVar;
        }

        [Conditional("DEBUG")]
        private static void validateIncludes(LambdaExpression map, string[] includes)
        {
            var allProps = includes.SelectMany(i => i.Split('.'));
            var repeats = allProps.GroupBy(p => p).Where(p => p.Count() > 1);
            if (repeats.Any())
            {
                throw new ArgumentException("include property ["+string.Join(",",repeats.Select(r=>r.Key))+"] is repeated");
            }
            var notNavs = allProps.Where(i => !_maps.ContainsKey(i)).ToList();
            if (notNavs.Any())
            {
                var bindNames = ((MemberInitExpression)map.Body).Bindings.ToHashSet(b => b.Member.Name);
                notNavs = notNavs.Where(n => !bindNames.Contains(n)).ToList();
                if (notNavs.Any())
                {
                    throw new ArgumentException("Could not find the include property named " + string.Join(",", notNavs));
                }
            }
        }

        private static IEnumerable<MapTreeNode> GetMaps(IEnumerable<string> includes)
        {
            foreach (string i in includes)
            {
                if (i.Contains('.') || _maps.ContainsKey(i)) //using containskey will result in 2 hashset lookups - assuming performance penalty irrelevant
                {
                    yield return MapTreeNode.Create(i);
                }
            }
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
            
        private class MapTreeNode
        {
            public readonly MapTreeNode Child;
            public readonly string PropertyName;
            public readonly LambdaExpression Map;

            internal MapTreeNode(string typeName, MapTreeNode child = null)
            {
                PropertyName = typeName;
                Child = child;
                Map = _maps[typeName];
            }

            public KeyValuePair<string, LambdaExpression> AsKeyValue()
            {
                return new KeyValuePair<string, LambdaExpression>(PropertyName, Map);
            }

            public static MapTreeNode Create(string typeName)
            {
                var typeNames = typeName.Split('.');

                MapTreeNode child = null;
                for (int i=typeNames.Length-1; i>=0; i--)
                {
                    child = new MapTreeNode(typeNames[i], child);
                }
                return child;
            }
            
            public KeyValuePair<string, LambdaExpression> MapChildren()
            {
                var stack = new Stack<MapTreeNode>();
                var child = this;
                do
                {
                    stack.Push(child);
                }
                while ((child = child.Child) != null);

                child = stack.Pop();
                if (stack.Count==0) { return child.AsKeyValue(); }
                LambdaExpression returnLambda = child.Map;
                do
                {
                    var parent = stack.Pop();
                    returnLambda = parent.Map.MapNavProperty(new[] { new KeyValuePair<string, LambdaExpression>(child.PropertyName, returnLambda) });
                    child = parent;
                } while (stack.Count > 0);

                return new KeyValuePair<string, LambdaExpression>(PropertyName,returnLambda);
            }

        }

    }
}
