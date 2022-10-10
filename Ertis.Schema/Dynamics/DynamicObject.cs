using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Ertis.Schema.Extensions;
using Newtonsoft.Json.Linq;

namespace Ertis.Schema.Dynamics
{
    public class DynamicObject : ICloneable
    {
        #region Properties

        private IDictionary<string, object> PropertyDictionary { get; init; }

        #endregion

        #region Indexer Overloading

        public object this[string path]
        {
            get => this.GetValue(path);
            set => this.SetValue(path, value);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        private DynamicObject()
        {
            this.PropertyDictionary = new Dictionary<string, object>();
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        public DynamicObject(object obj)
        {
            this.PropertyDictionary = obj.ToDictionary();
        }

        #endregion

        #region Methods
        
        public static DynamicObject Create(IDictionary<string, object> dictionary)
        {
            object model = dictionary.ToDynamic();
            return new DynamicObject
            {
                PropertyDictionary = model.ToDictionary()
            };
        }

        public static DynamicObject Parse(string json)
        {
            return new DynamicObject
            {
                PropertyDictionary = JToken.Parse(json).ToDictionary()
            };
        }
        
        public static DynamicObject Load(JToken jToken)
        {
            return new DynamicObject
            {
                PropertyDictionary = jToken.ToDictionary()
            };
        }
        
        public static T Cast<T>(dynamic obj) where T : class
        {
            if (obj == null)
            {
                return null;
            }

            var dynamicObject = new DynamicObject(obj);
            return dynamicObject.Deserialize<T>();
        }

        public IDictionary<string, object> ToDictionary()
        {
            return this.PropertyDictionary;
        }
        
        public string ToJson()
        {
            var dynamicObject = ToDynamic();
            return Newtonsoft.Json.JsonConvert.SerializeObject(dynamicObject);
        }
        
        // ReSharper disable once MemberCanBePrivate.Global
        public string Serialize()
        {
            return this.ToJson();
        }
        
        // ReSharper disable once MemberCanBePrivate.Global
        public T Deserialize<T>()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(this.ToJson());
        }
        
        public dynamic ToDynamic()
        {
            IDictionary<string, object> expandoDictionary = new ExpandoObject();
            foreach (var pair in this.PropertyDictionary)
            {
                if (pair.Value is IDictionary<string, object> childDictionary)
                {
                    expandoDictionary.Add(new KeyValuePair<string, object>(pair.Key, childDictionary.ToDynamic()));
                }
                else
                {
                    expandoDictionary.Add(pair);   
                }
            }

            dynamic dynamicObject = (ExpandoObject) expandoDictionary;
            return dynamicObject;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public object GetValue(string path)
        {
            return GetValueCore(path, this.PropertyDictionary);
        }
        
        // ReSharper disable once MemberCanBePrivate.Global
        public T GetValue<T>(string path)
        {
            var value = GetValueCore(path, this.PropertyDictionary);
            if (value is IDictionary<string, object> dictionary)
            {
                return Create(dictionary).Deserialize<T>();
            }
            else
            {
                return (T) value;   
            }
        }
        
        // ReSharper disable once MemberCanBePrivate.Global
        public bool TryGetValue(string path, out object value, out Exception exception)
        {
            try
            {
                value = GetValue(path);
                exception = null;
                return true;
            }
            catch (Exception ex)
            {
                value = default;
                exception = ex;
                return false;
            }
        }
        
        public bool TryGetValue<T>(string path, out T value, out Exception exception)
        {
            try
            {
                value = GetValue<T>(path);
                exception = null;
                return true;
            }
            catch (Exception ex)
            {
                value = default;
                exception = ex;
                return false;
            }
        }

        private static object GetValueCore(string path, IDictionary<string, object> dictionary)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path can not be null or empty!");
            }
            
            var segments = path.Split('.');
            var key = segments[0];
            if (dictionary.ContainsKey(key))
            {
                var value = dictionary[key];
                if (segments.Length > 1)
                {
                    if (value is IDictionary<string, object> subDictionary)
                    {
                        var subPath = string.Join(".", segments.Skip(1));
                        return GetValueCore(subPath, subDictionary);
                    }
                    else
                    {
                        throw new InvalidOperationException("Undefined");
                    }
                }
                else
                {
                    return value;
                }
            }
            else if (TryGetValueFromArray(key, dictionary, out var foundValue))
            {
                if (segments.Length > 1)
                {
                    if (foundValue is IDictionary<string, object> subDictionary)
                    {
                        var subPath = string.Join(".", segments.Skip(1));
                        return GetValueCore(subPath, subDictionary);
                    }
                    else
                    {
                        throw new InvalidOperationException("Undefined");
                    }
                }
                else
                {
                    return foundValue;
                }
            }
            else
            {
                throw new InvalidOperationException("Undefined");
            }
        }

        private static bool TryGetValueFromArray(string key, IDictionary<string, object> dictionary, out object foundValue)
        {
            if (key.Contains('[') && key.EndsWith(']'))
            {
                var indexerStartIndex = key.IndexOf('[');
                var indexerCloseIndex = key.IndexOf(']');

                var originalKey = key.Substring(0, indexerStartIndex);
                if (dictionary.ContainsKey(originalKey))
                {
                    var value = dictionary[originalKey];
                    if (value is Array array)
                    {
                        var indexStr = key.Substring(indexerStartIndex + 1, indexerCloseIndex - indexerStartIndex - 1);
                        if (int.TryParse(indexStr, out var index))
                        {
                            if (index < array.Length)
                            {
                                foundValue = array.GetValue(index);
                                return true;
                            }
                            else
                            {
                                throw new InvalidOperationException($"Out of range (length: {array.Length}, index: {index})");
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException($"Array index is not valid integer ('{indexStr}')");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Indexed node is not an array");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Undefined");
                }
            }

            foundValue = null;
            return false;
        }
        
        // ReSharper disable once MemberCanBePrivate.Global
        public void SetValue(string path, object value, bool createIfNotExist = false)
        {
            SetValueCore(path, value, this.PropertyDictionary, createIfNotExist);
        }
        
        public bool TrySetValue(string path, object value, out Exception exception, bool createIfNotExist = false)
        {
            try
            {
                SetValue(path, value, createIfNotExist);
                exception = null;
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }
        
        private static void SetValueCore(string path, object obj, IDictionary<string, object> dictionary, bool createIfNotExist)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path can not be null or empty!");
            }
            
            var segments = path.Split('.');
            var key = segments[0];
            if (dictionary.ContainsKey(key))
            {
                var value = dictionary[key];
                if (segments.Length > 1)
                {
                    if (value is IDictionary<string, object> subDictionary)
                    {
                        var subPath = string.Join(".", segments.Skip(1));
                        SetValueCore(subPath, obj, subDictionary, createIfNotExist);
                    }
                    else
                    {
                        throw new InvalidOperationException("Undefined");
                    }
                }
                else
                {
                    dictionary[key] = obj;
                }
            }
            else if (TrySetValueOnArray(path, dictionary, obj))
            {
                // NOP
            }
            else if (createIfNotExist)
            {
                if (segments.Length > 1)
                {
                    var subPath = string.Join(".", segments.Skip(1));
                    SetValueCore(subPath, obj, new Dictionary<string, object>(), true);
                }
                else
                {
                    dictionary.Add(key, obj);
                }
            }
            else
            {
                throw new InvalidOperationException("Undefined");
            }
        }

        private static bool TrySetValueOnArray(string key, IDictionary<string, object> dictionary, object setValue)
        {
            var indexerStartIndex = key.IndexOf('[');
            var indexerCloseIndex = key.IndexOf(']');
            
            if (indexerStartIndex > 0 && indexerCloseIndex > 0 && indexerStartIndex < indexerCloseIndex)
            {
                var originalKey = key.Substring(0, indexerStartIndex);
                if (dictionary.ContainsKey(originalKey))
                {
                    var value = dictionary[originalKey];
                    if (value is Array array)
                    {
                        var indexStr = key.Substring(indexerStartIndex + 1, indexerCloseIndex - indexerStartIndex - 1);
                        if (int.TryParse(indexStr, out var index))
                        {
                            if (index < array.Length)
                            {
                                var indexerPath = $"{originalKey}[{index}]";
                                var arrayItem = array.GetValue(index);
                                if (key.Length > indexerPath.Length && arrayItem is IDictionary<string, object> subDictionary)
                                {
                                    SetValueCore(key[indexerPath.Length..].TrimStart('.'), setValue, subDictionary, false);
                                    return true;
                                }
                                else
                                {
                                    array.SetValue(setValue, index);
                                    return true;   
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException($"Out of range (length: {array.Length}, index: {index})");
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException($"Array index is not valid integer ('{indexStr}')");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Indexed node is not an array");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Undefined");
                }
            }

            return false;
        }
        
        public void RemoveProperty(string path)
        {
            RemovePropertyCore(path, this.PropertyDictionary);
        }
        
        private static void RemovePropertyCore(string path, IDictionary<string, object> dictionary)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path can not be null or empty!");
            }
            
            var segments = path.Split('.');
            var key = segments[0];
            if (dictionary.ContainsKey(key))
            {
                var value = dictionary[key];
                if (segments.Length > 1)
                {
                    if (value is IDictionary<string, object> subDictionary)
                    {
                        var subPath = string.Join(".", segments.Skip(1));
                        RemovePropertyCore(subPath, subDictionary);
                    }
                }
                else
                {
                    dictionary.Remove(key);
                }
            }
        }
        
        public bool ContainsProperty(string path)
        {
            if (!this.TryGetValue(path, out _, out var exception))
            {
                if (exception is InvalidOperationException && exception.Message == "Undefined")
                {
                    return false;
                }
                else
                {
                    throw exception;
                }
            }

            return true;
        }

        #endregion

        #region ICloneable

        public object Clone()
        {
            return Parse(this.ToJson());
        }

        #endregion
    }
}