using Newtonsoft.Json.Linq;
using SM.Dto.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.Dto
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
            /*
            Debug.WriteLine(string.Join("\r\n",
                                from r in relationships
                                where r.Value.Properties[1] == null
                                let p = r.Value.Properties[0]
                                select string.Format("{0}: {1}.{2}", r.Key, p.TypeName, p.PropertyName)));
            */
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

            return relationships.Values.Select(v => v.Properties.Single())
                .ToLookup(k => k.TypeName, v => v.PropertyName.PascalToCamelCase()).ToDictionary(k=>k.Key, v=>v.AsEnumerable());
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
                Properties = new EdmxProperty[2];
                Properties[0] = propName;
            }
            public EdmxProperty[] Properties { get; private set; }

            public void AddProperty(EdmxProperty propName)
            {
                Debug.Assert(Properties[1] == null);
                Properties[1] = propName;
            }

            public void SetSingleMultiplicityType(string typeName)
            {
                int i;
                for (i=0; i < Properties.Length; i++)
                {
                    //if this throws, it is because the relationships in the entity model are not configured to be bi-directional
                    //modify this accordingly, but I want it to throw until I have my entity model sorted out
                    if (Properties[i].TypeName == typeName)
                    {
                        break;
                    } 
                }
                
                var newProperties = new EdmxProperty[1];
                Array.Copy(Properties, i, newProperties,0, 1); 
                Properties = newProperties;
            }

        }
    }
}
