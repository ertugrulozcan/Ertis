using System;
using System.Linq;
using Ertis.Schema.Dynamics;
using Ertis.Schema.Types;
using Ertis.Schema.Types.Primitives;

namespace Ertis.Schema.Extensions
{
    public static class DynamicObjectExtensions
    {
        #region Methods

        public static void EnsureFieldTypesAccuracy(this DynamicObject model, ISchema contentType)
        {
            foreach (var fieldInfo in contentType.Properties)
            {
                EnsureFieldTypesAccuracyCore(model, contentType, fieldInfo);
            }
        }

        private static void EnsureFieldTypesAccuracyCore(DynamicObject model, ISchema contentType, IFieldInfo fieldInfo)
        {
            try
            {
                switch (fieldInfo.Type)
                {
                    // Date & DateTime's
                    case FieldType.date or FieldType.datetime:
                        ResetFieldValue<DateTime?>(model, contentType, fieldInfo, oldValue =>
                        {
                            if (oldValue != null && DateTime.TryParse(oldValue.ToString(), out var dateTime))
                            {
                                return fieldInfo.Type == FieldType.datetime ? dateTime.ToLocalTime() : dateTime.Date.ToLocalTime();
                            }

                            return null;
                        });
                        break;
                    case FieldType.@object:
                    {
                        if (fieldInfo is ObjectFieldInfo objectFieldInfo)
                        {
                            foreach (var property in objectFieldInfo.Properties)
                            {
                                EnsureFieldTypesAccuracyCore(model, contentType, property);    
                            }
                        }

                        break;
                    }
                    case FieldType.array:
                    {
                        if (fieldInfo is ArrayFieldInfo arrayFieldInfo)
                        {
                            EnsureFieldTypesAccuracyCore(model, contentType, arrayFieldInfo.ItemSchema);
                        }

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void ResetFieldValue<T>(DynamicObject model, ISchema contentType, IFieldInfo fieldInfo, Func<object, T> action)
        {
            try
            {
                if (!string.IsNullOrEmpty(fieldInfo.Path))
                {
                    var fieldPath = fieldInfo.Path;
                    var segments = fieldInfo.Path.Split('.');
                    if (segments.Length > 1 && segments[0] == contentType.Slug)
                    {
                        fieldPath = string.Join('.', segments.Skip(1));
                    }
                    
                    if (model.ContainsProperty(fieldPath))
                    {
                        var oldValue = model.GetValue(fieldPath);
                        var newValue = action(oldValue);
                        model.TrySetValue(fieldPath, newValue, out _);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        #endregion
    }
}