using System;
using System.Collections.Generic;
using System.Linq;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types;
using Ertis.Schema.Types.Primitives;

namespace Ertis.Schema.Extensions
{
    public static class SchemaExtensions
    {
        #region Methods
        
        public static IFieldInfo FindField(this ISchema schema, string path)
        {
            return FindFieldCore(schema.Properties, path);
        }
        
        private static IFieldInfo FindFieldCore(IEnumerable<IFieldInfo> properties, string path)
        {
            foreach (var property in properties)
            {
                var found = FindFieldCore(property, path);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }
        
        private static IFieldInfo FindFieldCore(IFieldInfo property, string path)
        {
            if (property.Type == FieldType.@object && property is ObjectFieldInfo objectFieldInfo)
            {
                return FindFieldCore(objectFieldInfo.Properties, path);
            }
            else if (property.Type == FieldType.array && property is ArrayFieldInfo arrayFieldInfo)
            {
                return FindFieldCore(arrayFieldInfo.ItemSchema, path);
            }
            else
            {
                return property.Path == path ? property : null;
            }
        }

        public static bool CheckPropertiesUniqueness(this ISchema schema, out Exception exception)
        {
            var fieldInfos = schema.Properties;
            var distinctCount = fieldInfos.Select(x => x.Name).Distinct().Count();
            if (fieldInfos.Count != distinctCount)
            {
                if (schema is IFieldInfo fieldInfo)
                {
                    exception = new FieldValidationException("Duplicate property declaration. Property names are must be unique.", fieldInfo);    
                }
                else
                {
                    exception = new SchemaValidationException("Duplicate property declaration. Property names are must be unique.");
                }
                
                return false;
            }

            foreach (var fieldInfo in fieldInfos)
            {
                if (fieldInfo is ISchema subObjectSchema)
                {
                    var isValid = CheckPropertiesUniqueness(subObjectSchema, out exception);
                    if (!isValid)
                    {
                        return false;
                    }
                }
            }
            
            exception = null;
            return true;
        }

        public static IEnumerable<IFieldInfo> GetUniqueProperties(this ISchema schema)
        {
            var uniqueProperties = new List<IFieldInfo>();
            var fieldInfos = schema.Properties;
            foreach (var fieldInfo in fieldInfos)
            {
                uniqueProperties.AddRange(GetUniqueProperties(fieldInfo));
            }

            return uniqueProperties;
        }
        
        private static IEnumerable<IFieldInfo> GetUniqueProperties(IFieldInfo fieldInfo)
        {
            var uniqueProperties = new List<IFieldInfo>();
            switch (fieldInfo)
            {
                case IPrimitiveType { IsUnique: true }:
                    uniqueProperties.Add(fieldInfo);
                    break;
                case ObjectFieldInfo objectFieldInfo:
                {
                    foreach (var property in objectFieldInfo.Properties)
                    {
                        uniqueProperties.AddRange(GetUniqueProperties(property));
                    }

                    break;
                }
                case ArrayFieldInfo arrayFieldInfo:
                    uniqueProperties.AddRange(GetUniqueProperties(arrayFieldInfo.ItemSchema));
                    break;
            }

            return uniqueProperties;
        }

        #endregion
    }
}