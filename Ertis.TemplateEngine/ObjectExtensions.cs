using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Ertis.TemplateEngine
{
    internal static class ObjectExtensions
    {
        #region Methods

        public static IDictionary<string, object> ToDictionary(this object model)
        {
            var dictionary = new Dictionary<string, object>();
            
            var jObject = JObject.FromObject(model);
            foreach (var childToken in jObject.Children())
            {
                if (childToken is JProperty jProperty)
                {
                    var propertyName = jProperty.Name;
                    dynamic dynamicObject = jProperty.Value;
                    dictionary.Add(propertyName, ToDictionaryCore(dynamicObject));
                }
            }

            return dictionary;
        }

        private static object ToDictionaryCore(this object model)
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
                    return jObject.Children().ToDictionary(childToken => childToken.Path, ToDictionaryCore);
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

        #endregion
    }
}