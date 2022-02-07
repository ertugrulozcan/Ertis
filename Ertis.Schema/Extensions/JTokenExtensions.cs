using System.Collections.Generic;
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
        
        #endregion
    }
}