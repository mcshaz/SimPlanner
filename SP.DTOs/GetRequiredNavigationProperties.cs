using Newtonsoft.Json.Linq;
using SP.Dto.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Dto
{
    static class EdmxExplorer
    {

        internal static Dictionary<string,IEnumerable<string>> GetRequiredNavProperties(string edmx)
        {
            var metadata = JObject.Parse(edmx)["schema"];
            var relationships = new Dictionary<string, NavigationProperty>();
            foreach (var node in metadata["entityType"])
            {
                var typeName = (string)node["name"];
                var navProp = node["navigationProperty"];
                if (navProp == null) { continue; }
                IEnumerable<JToken> navPropEnumerator = navProp as JArray ?? (IEnumerable<JToken>)new[] { navProp };

                foreach (var nav in navPropEnumerator)
                {
                    string relationship = ((string)nav["relationship"]).ExtractAfterLastDot(); //remove self.
                    var ep = new EdmxProperty { TypeName = typeName, PropertyName = (string)nav["name"] };
                    NavigationProperty n;
                    if (relationships.TryGetValue(relationship, out n))
                    {

                        n.AddProperty(ep);
                    }
                    else
                    {
                        relationships.Add(relationship, new NavigationProperty(ep));
                    }
                }
            }
            //uncomment the line below to find all entity relations which are 1 way (i.e. you have forgotten to map between both entities)
            
            Debug.WriteLine(string.Join("\r\n",
                                from r in relationships
                                let p = r.Value.UnmatchedProperty
                                where p != null
                                select string.Format("{0}: {1}.{2}", r.Key, p.TypeName, p.PropertyName)));
            
            foreach (var node in metadata["association"])
            {
                var single = node["end"].FirstOrDefault(n => (string)n["multiplicity"] == "1");
                var name = (string)node["name"];
                //NOT dealing with 1..1 relationships as this is an edge case with the current DB 
                if (single==null)
                {
                    relationships.Remove(name);
                }
                else
                {
                    relationships[name].SetSingleMultiplicityType(((string)single["type"]).ExtractAfterLastDot());
                }
            }

            return relationships.Values.Select(v => v.RequiredProperty)
                .ToLookup(k => k.TypeName, v => v.PropertyName.PascalToCamelCase())
                .ToDictionary(k=>k.Key, v=>v.AsEnumerable());
            //could return the lookup directly, but newtonsoft serialisation would require a custom converter
                
        }

        private static string ExtractAfterLastDot(this string inStr)
        {
            int i = inStr.LastIndexOf('.');
            if (++i == 0) { return inStr; }
            return inStr.Substring(i);
        }
        
        private class EdmxProperty
        {
            public string TypeName { get; set; }
            public string PropertyName { get; set; }
        }

        private class NavigationProperty
        {
            public NavigationProperty(EdmxProperty propName)
            {
                _properties = new EdmxProperty[2];
                _properties[0] = propName;
            }
            readonly EdmxProperty[] _properties;
            public EdmxProperty RequiredProperty { get; private set; }

            public void AddProperty(EdmxProperty propName)
            {
                Debug.Assert(_properties[1] == null);
                _properties[1] = propName;
            }

            public EdmxProperty UnmatchedProperty
            {
                get
                {
                    if (_properties[1] != null) { return null; }
                    return _properties[0];
                }
            }

            public void SetSingleMultiplicityType(string typeName)
            {
                if (_properties[1] == null)
                {
                    var unmatched = _properties[0];
                    Debug.WriteLine("null reference - PropertyName:'{0}' TypeName:'{1}'", unmatched.PropertyName, unmatched.TypeName);
                }
                int i;
                for (i=0; i < _properties.Length; i++)
                {
                    //if this throws, it is because the relationships in the entity model are not configured to be bi-directional
                    //modify this accordingly, but I want it to throw until I have my entity model sorted out

                    if (_properties[i].TypeName == typeName)
                    {
                        break;
                    } 
                }

                RequiredProperty = _properties[1 - i];
            }

        }
    }
}
