using System;
using System.Collections.Generic;
using System.Linq;
using Ertis.Schema.Dynamics;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types;
using Ertis.Schema.Types.CustomTypes;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Validation;

namespace Ertis.Schema.Extensions
{
    public static class SchemaExtensions
    {
        #region Validation Methods

        public static void Validate(this ISchema schema, out Exception exception)
        {
            schema.ValidateProperties(out exception);
        }

        internal static bool ValidateProperties(this ISchema schema, out Exception exception)
        {
            if (schema.Properties == null)
            {
                exception = new SchemaValidationException($"Properties field is required for schema objects ({schema.Slug})");
                return false;
            }

            foreach (var fieldInfo in schema.Properties)
            {
                if (!fieldInfo.ValidateSchema(out exception))
                {
                    return false;
                }
            }
            
            schema.CheckPropertiesUniqueness(out exception);
            
            var uniqueProperties = schema.GetUniqueProperties();
            foreach (var uniqueProperty in uniqueProperties)
            {
                if (uniqueProperty.IsAnArrayItem(out _))
                {
                    exception = new SchemaValidationException($"The unique constraints could not use in arrays. Use the 'uniqueBy' feature instead of. ('{uniqueProperty.Name}')");
                    
                    return false;
                }
            }
            
            return exception == null;
        }

        public static bool ValidateData(this ISchema schema, DynamicObject model, IValidationContext validationContext)
        {
            try
            {
                ObjectFieldInfo rootObjectFieldInfo;
                if (schema is ObjectFieldInfo objectFieldInfo)
                {
                    rootObjectFieldInfo = objectFieldInfo;
                }
                else
                {
                    rootObjectFieldInfo = new ObjectFieldInfo(schema.Properties)
                    {
                        Name = schema.Slug
                    };
                }
                
                var isValidContent = rootObjectFieldInfo.ValidateContent(model, validationContext);

                schema.SetDefaultValues(model);
                schema.SetConstants(model);
                schema.SetFormatPatterns(model);
                
                return isValidContent;
            }
            catch (FieldValidationException ex)
            {
                validationContext.Errors.Add(ex);
                return false;
            }
        }

        #endregion
        
        #region Schema Tree Methods
        
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

        #endregion

        #region Uniqueness Methods

        private static bool CheckPropertiesUniqueness(this ISchema schema, out Exception exception)
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

        #endregion
        
        #region Unique Property Methods

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
        
        #region Reference Content Methods

        public static IEnumerable<ReferenceFieldInfo> GetReferenceProperties(this ISchema schema)
        {
            var referenceProperties = new List<ReferenceFieldInfo>();
            var fieldInfos = schema.Properties;
            foreach (var fieldInfo in fieldInfos)
            {
                referenceProperties.AddRange(GetReferenceProperties(fieldInfo));
            }

            return referenceProperties;
        }
        
        private static IEnumerable<ReferenceFieldInfo> GetReferenceProperties(IFieldInfo fieldInfo)
        {
            var referenceProperties = new List<ReferenceFieldInfo>();
            
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (fieldInfo.Type)
            {
                case FieldType.reference:
                    referenceProperties.Add(fieldInfo as ReferenceFieldInfo);
                    break;
                case FieldType.@object when fieldInfo is ObjectFieldInfo objectFieldInfo:
                {
                    foreach (var property in objectFieldInfo.Properties)
                    {
                        referenceProperties.AddRange(GetReferenceProperties(property));
                    }

                    break;
                }
                case FieldType.@object when fieldInfo is ArrayFieldInfo arrayFieldInfo:
                    referenceProperties.AddRange(GetReferenceProperties(arrayFieldInfo.ItemSchema));
                    break;
            }

            return referenceProperties;
        }

        #endregion
        
        #region DefaultValue Methods

        private static void SetDefaultValues(this ISchema schema, DynamicObject model)
        {
            SetDefaultValues(schema.Properties, model);
        }
        
        private static void SetDefaultValues(IEnumerable<IFieldInfo> properties, DynamicObject model)
        {
            foreach (var fieldInfo in properties)
            {
                SetDefaultValues(fieldInfo, model);
            }
        }
        
        private static void SetDefaultValues(IFieldInfo fieldInfo, DynamicObject model)
        {
            if (fieldInfo is IHasDefault hasDefault)
            {
                var defaultValue = hasDefault.GetDefaultValue();
                if (defaultValue != null)
                {
                    var path = fieldInfo.Path;
                    var segments = path.Split('.');
                    if (segments.Length > 1)
                    {
                        path = string.Join(".", segments.Skip(1));
                    }

                    if (!model.TryGetValue(path, out var currentValue, out _) || currentValue == null)
                    {
                        model.TrySetValue(path, defaultValue, out _, true);
                    }
                }
            }
        }

        #endregion
        
        #region Constant Methods

        private static void SetConstants(this ISchema schema, DynamicObject model)
        {
            SetConstants(schema.Properties, model);
        }

        private static void SetConstants(IEnumerable<IFieldInfo> properties, DynamicObject model)
        {
            foreach (var fieldInfo in properties)
            {
                SetConstants(fieldInfo, model);
            }
        }
        
        private static void SetConstants(IFieldInfo fieldInfo, DynamicObject model)
        {
            if (fieldInfo is ConstantFieldInfo constantFieldInfo)
            {
                var path = fieldInfo.Path;
                var segments = path.Split('.');
                if (segments.Length > 1)
                {
                    path = string.Join(".", segments.Skip(1));
                }

                model.TrySetValue(path, constantFieldInfo.Value, out _, true);
            }
        }
        
        #endregion
        
        #region FormatPattern Methods

        private static void SetFormatPatterns(this ISchema schema, DynamicObject model)
        {
            SetFormatPatterns(schema.Properties, model);
        }

        private static void SetFormatPatterns(IEnumerable<IFieldInfo> properties, DynamicObject model)
        {
            foreach (var fieldInfo in properties)
            {
                SetFormatPatterns(fieldInfo, model);
            }
        }
        
        private static void SetFormatPatterns(IFieldInfo fieldInfo, DynamicObject model)
        {
            if (fieldInfo is StringFieldInfo stringFieldInfo)
            {
                string value = null;
                if (stringFieldInfo.CurrentObject != null && !string.IsNullOrEmpty(stringFieldInfo.CurrentObject.ToString()))
                {
                    value = stringFieldInfo.CurrentObject.ToString();
                }
                else if (!string.IsNullOrEmpty(stringFieldInfo.FormatPattern))
                {
                    value = stringFieldInfo.Format(model);
                }
                
                if (!string.IsNullOrEmpty(value))
                {
                    var path = fieldInfo.Path;
                    var segments = path.Split('.');
                    if (segments.Length > 1)
                    {
                        path = string.Join(".", segments.Skip(1));
                    }

                    model.TrySetValue(path, value, out _, true);
                }
            }
        }
        
        #endregion
    }
}