using Jint;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SM.Dto.Utilities;
using SM.Metadata;
using SM.Metadata.CustomValidators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SM.Dto
{
    public class MetaDataStrings
    {
        public string Breeze { get; set; }
        public string RequiredNavProperties { get; set; }
    }
    public static class MedSimDtoMetadata
    {
        const string breezeJsPath = @"C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimManager\SM.Web\Scripts\breeze.min.js";
        public static string GetBreezeMetadata(string edmx, bool pretty = false)
        {
            var engine = new Engine().Execute("var setInterval;var setTimeout = setInterval = function(){}"); //if using an engine like V8.NET, would not be required - not part of DOM spec
            engine.Execute(File.ReadAllText(breezeJsPath));
            engine.Execute("breeze.NamingConvention.camelCase.setAsDefault();" + //mirror here what you are doing in the client side code
                           "breeze.DataType.DateTime.defaultValue = '';" + //personal preference
                           "var edmxMetadataStore = new breeze.MetadataStore();" +
                           "edmxMetadataStore.importMetadata(JSON.stringify(" + edmx + "));" +
                           "edmxMetadataStore.exportMetadata();");
            var exportedMeta = JObject.Parse(engine.GetCompletionValue().AsString());
            AddValidators(exportedMeta);
            return exportedMeta.ToString(pretty ? Formatting.Indented : Formatting.None);
        }

        public static MetaDataStrings GetAllMetadata(bool pretty = false)
        {
            string edmx = MedSimDtoRepository.GetEdmxMetadata();
            //File.WriteAllText("dtoMetadata.edmx", edmx);
            return new MetaDataStrings
            {
                RequiredNavProperties = JsonConvert.SerializeObject(EdmxExplorer.GetRequiredNavProperties(edmx), pretty?Formatting.Indented:Formatting.None),
                Breeze = GetBreezeMetadata(edmx, pretty)
            };
        }

        //http://stackoverflow.com/questions/26570638/how-to-add-extend-breeze-entity-types-with-metadata-pulled-from-property-attribu
        static void AddValidators(JObject metadata)
        {
            Assembly thisAssembly = typeof(ParticipantDto).Assembly; //any type in the assembly containing the Breeze entities.
            var attrValDict = GetValDictionary();
            var unaccountedVals = new HashSet<string>();
            var intTypes = new[] { typeof(byte), typeof(Int16), typeof(Int32), typeof(Int64) };
            foreach (var breezeEntityType in metadata["structuralTypes"])
            {
                string shortEntityName = breezeEntityType["shortName"].ToString();
                string typeName = breezeEntityType["namespace"].ToString() + '.' + shortEntityName;
                Type entityType = thisAssembly.GetType(typeName, true);
                Type metaTypeFromAttr = ((MetadataTypeAttribute)entityType.GetCustomAttributes(typeof(MetadataTypeAttribute), false).Single()).MetadataClassType;

                foreach (var breezePropertyInfo in breezeEntityType["dataProperties"])
                {
                    string propName = breezePropertyInfo["name"].ToString();
                    propName = propName.CamelToPascalCase(); //IF client using breeze.NamingConvention.camelCase & server using PascalCase
                    var propInfo = metaTypeFromAttr.GetProperty(propName);

                    if (propInfo == null)
                    {
                        Debug.WriteLine("No metadata property attributes available for " + breezePropertyInfo["dataType"] + " "+ shortEntityName +'.' + propName);
                        continue;
                    }

                    var validators = breezePropertyInfo["validators"].Select(bp => bp.ToObject<Dictionary<string, object>>()).ToDictionary(key => (string)key["name"]);

                    //usingMetaProps purely on property name - could also use the DTO object itself
                    //if metadataType not found, or in reality search the entity framework entity
                    //for properties with the same name (that is certainly how I am mapping)


                    foreach (Attribute attr in propInfo.GetCustomAttributes())
                    {
                        Type t = attr.GetType();
                        if (t.Namespace == "System.ComponentModel.DataAnnotations.Schema") {
                            continue;
                        } else if (t == typeof(DefaultValueAttribute))
                        {
                            var def = (DefaultValueAttribute)attr;
                            breezePropertyInfo["defaultValue"] = JToken.FromObject(def.Value);
                        }
                        Func<Attribute, Dictionary<string,object>> getVal;
                        if (attrValDict.TryGetValue(t, out getVal))
                        {
                            var validatorsFromAttr = getVal(attr);
                            if (validatorsFromAttr != null)
                            {
                                string jsValidatorName = (string)validatorsFromAttr["name"];
                                Dictionary<string, object> existingVals;
                                if (validators.TryGetValue(jsValidatorName, out existingVals))
                                {
                                    existingVals.AddOrOverwrite(validatorsFromAttr);
                                }
                                else
                                {
                                    validators.Add(jsValidatorName, validatorsFromAttr);
                                }
                            }
                        }
                        else
                        {
                            unaccountedVals.Add(t.FullName);
                        }

                    }
                    if (validators.ContainsKey("stringLength") )
                    {
                        validators.Remove("maxLength");
                    }
                    Dictionary<string, object> rangeVal;
                    if (intTypes.Contains(propInfo.PropertyType) && validators.TryGetValue("numericRange", out rangeVal))
                    {
                        validators.Remove(propInfo.PropertyType.Name.PascalToCamelCase());
                    }
                    breezePropertyInfo["validators"] = JToken.FromObject(validators.Values);
                }
            }
            foreach (var u in unaccountedVals)
            {
                Debug.WriteLine("unaccounted attribute:" + u);
            }
        }

        static Dictionary<Type, Func<Attribute, Dictionary<string, object>>> GetValDictionary()
        {
            var ignore = new Func<Attribute, Dictionary<string, object>>(x => null);
            return new Dictionary<Type, Func<Attribute, Dictionary<string, object>>>
            {
                [typeof(RequiredAttribute)] = x => {
                    var returnVar = new Dictionary<string, object>
                    {
                        ["name"] = "required"
                        //["message"] = ((RequiredAttribute)x).ErrorMessage
                    };
                    if (((RequiredAttribute)x).AllowEmptyStrings)
                    {
                        returnVar.Add("allowEmptyStrings", true);
                    }
                    return returnVar;
                },
                [typeof(EmailAddressAttribute)] = x => new Dictionary<string, object>
                {
                    ["name"] = "emailAddress",
                },
                [typeof(PhoneAttribute)] = x => new Dictionary<string, object>
                {
                    ["name"] = "phone",
                },
                [typeof(RegularExpressionAttribute)] = x => new Dictionary<string, object>
                {
                    ["name"] = "regularExpression",
                    ["expression"] = ((RegularExpressionAttribute)x).Pattern
                },
                [typeof(StringLengthAttribute)] = x => {
                    var sl = (StringLengthAttribute)x;
                    return GetStrLenDictionary(sl.MaximumLength, sl.MinimumLength);
                },
                [typeof(MaxLengthAttribute)] = x => GetStrLenDictionary(((MaxLengthAttribute)x).Length),
                [typeof(UrlAttribute)] = x => new Dictionary<string, object>
                {
                    ["name"] = "url"
                },
                [typeof(CreditCardAttribute)] = x=> new Dictionary<string, object>
                {
                    ["name"] = "creditCard"
                },
                [typeof(FixedLengthAttribute)] = x => //note this is one of my attributes to force fixed length
                {
                    var len = ((FixedLengthAttribute)x).Length;
                    return GetStrLenDictionary(len, len);
                },
                [typeof(RangeAttribute)] = x => {
                    var ra = (RangeAttribute)x;
                    return new Dictionary<string, object>
                    {
                        ["name"] = "numericRange",
                        ["min"] = ra.Minimum,
                        ["max"] = ra.Maximum
                    }; 
                },
                [typeof(PersonFullNameAttribute)] = x=> new Dictionary<string, object>
                {
                    ["name"] = "personFullName",
                    ["minNames"] = PersonFullNameAttribute.MinNames,
                    ["maxNames"] = PersonFullNameAttribute.MaxNames,
                    ["minNameLength"] = PersonFullNameAttribute.MinNameLength
                },
                [typeof(KeyAttribute)] = ignore
            };
        }

        static Dictionary<string,object> GetStrLenDictionary(int maxLength, int minLength = 0)
        {
            if (minLength == 0)
            {
                return new Dictionary<string, object>
                {
                    ["name"] = "maxLength",
                    ["maxLength"] = maxLength
                };
            }
            return new Dictionary<string, object>
            {
                ["name"] = "stringLength",
                ["minLength"] = minLength,
                ["maxLength"] = maxLength
            };
        }

        static void AddOrOverwrite<K,V>(this Dictionary<K,V> oldValues, Dictionary<K,V> newValues)
        {
            foreach (KeyValuePair<K,V> kv in newValues)
            {
                if (oldValues.ContainsKey(kv.Key))
                {
                    oldValues[kv.Key] = kv.Value;
                }
                else
                {
                    oldValues.Add(kv.Key, kv.Value);
                }
            }

        }
    }
}
