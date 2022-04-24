using System.Collections.Generic;
using Ertis.Schema.Dynamics;

namespace Ertis.Schema.Extensions
{
    public static class DynamicObjectExtensions
    {
        #region Methods

        public static DynamicObject Merge(this DynamicObject dynamicObject1, DynamicObject dynamicObject2)
        {
            return Merge(dynamicObject1.ToDictionary(), dynamicObject2.ToDictionary());
        }
        
        private static DynamicObject Merge(IDictionary<string, object> dictionary1, IDictionary<string, object> dictionary2)
        {
            var dictionary = new Dictionary<string, object>();

            var visitedProperties = new List<string>();
            foreach (var (propertyName, propertyValue) in dictionary1)
            {
                dictionary.Add(propertyName, propertyValue);
                if (dictionary2.ContainsKey(propertyName))
                {
                    if (propertyValue is IDictionary<string, object> subDictionary1 && dictionary2[propertyName] is IDictionary<string, object> subDictionary2)
                    {
                        dictionary[propertyName] = Merge(subDictionary1, subDictionary2).ToDictionary();
                    }
                    else
                    {
                        dictionary[propertyName] = dictionary2[propertyName];
                    }
                    
                    visitedProperties.Add(propertyName);
                }
            }

            foreach (var (propertyName, propertyValue) in dictionary2)
            {
                if (!visitedProperties.Contains(propertyName))
                {
                    dictionary[propertyName] = propertyValue;
                }
            }
            
            return DynamicObject.Create(dictionary);
        }

        #endregion
    }
}