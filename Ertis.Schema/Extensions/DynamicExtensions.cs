using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Ertis.Schema.Extensions
{
    public static class DynamicExtensions
    {
        #region Methods

        public static IDictionary<string, object> ToDictionary(this object model)
        {
            var jObject = JObject.FromObject(model);
            return jObject.ToDictionary();
        }

        internal static object ToDictionaryCore(this object model)
        {
            var jToken = JToken.FromObject(model);
            switch (jToken)
            {
                case JProperty jProperty:
                {
                    if (jProperty.Value is JValue jValue)
                    {
                        return jValue.Value;
                    }
                    else
                    {
                        dynamic dynamicObject = jProperty.Value;
                        return ToDictionaryCore(dynamicObject);
                    }
                }
                case JValue jValue:
                {
                    return jValue.Value;
                }
                case JObject jObject:
                {
                    return jObject.Children().ToDictionary(childToken => childToken.GetFullPath(), ToDictionaryCore);
                }
                case JArray jArray:
                {
                    return jArray.Select(ToDictionaryCore).ToArray();
                }
                default:
                {
                    throw new Exception("Unknown json node in ToDictionaryCore");
                }
            }
        }
        
        public static dynamic ToDynamic(this IDictionary<string, object> dictionary)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var pair in dictionary)
            {
                if (pair.Value is IDictionary<string, object> childDictionary)
                {
                    expando.Add(new KeyValuePair<string, object>(pair.Key, childDictionary.ToDynamic()));
                }
                else
                {
                    expando.Add(pair);   
                }
            }
            
            return (ExpandoObject) expando;
        }

        #endregion
    }
}