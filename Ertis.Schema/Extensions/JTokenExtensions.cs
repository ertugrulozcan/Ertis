using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Ertis.Schema.Extensions
{
    public static class JTokenExtensions
    {
        #region Methods

        public static IDictionary<string, object> ToDictionary(this JToken jToken)
        {
            var dictionary = new Dictionary<string, object>();
            
            foreach (var childToken in jToken.Children())
            {
                if (childToken is JProperty jProperty)
                {
                    var propertyName = jProperty.Name;
                    dynamic dynamicObject = jProperty.Value;
                    dictionary.Add(propertyName, DynamicExtensions.ToDictionaryCore(dynamicObject));
                }
            }

            return dictionary;
        }
        
        public static string GetFullPath(this JToken jToken)
        {
            if (jToken.Path.StartsWith("['") && jToken.Path.EndsWith("']"))
            {
                return string.Join('.', jToken.AncestorsAndSelf()
                    .OfType<JProperty>()
                    .Select(p => p.Name)
                    .Reverse());
            }

            return jToken.Path;
        }
        
        #endregion
    }
}