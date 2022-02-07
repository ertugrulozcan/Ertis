using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Ertis.Schema.Extensions;
using Newtonsoft.Json.Linq;

namespace Ertis.Schema.Dynamics
{
    public class DynamicObject
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
            return new DynamicObject
            {
                PropertyDictionary = dictionary
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

        public IDictionary<string, object> ToDictionary()
        {
            return this.PropertyDictionary;
        }
        
        public string ToJson()
        {
            var dynamicObject = ToDynamic();
            return Newtonsoft.Json.JsonConvert.SerializeObject(dynamicObject);
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
            return (T) value;
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
            else
            {
                throw new InvalidOperationException("Undefined");
            }
        }
        
        // ReSharper disable once MemberCanBePrivate.Global
        public void SetValue(string path, object value, bool createIfNotExist = false)
        {
            SetValueCore(path, value, this.PropertyDictionary, createIfNotExist);
        }
        
        public bool TrySetValue(string path, object value, out Exception exception)
        {
            try
            {
                SetValue(path, value);
                exception = null;
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }
        
        private static void SetValueCore(string path, object value, IDictionary<string, object> dictionary, bool createIfNotExist)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path can not be null or empty!");
            }
            
            var segments = path.Split('.');
            var key = segments[0];
            if (dictionary.ContainsKey(key))
            {
                if (segments.Length > 1)
                {
                    if (value is IDictionary<string, object> subDictionary)
                    {
                        var subPath = string.Join(".", segments.Skip(1));
                        SetValueCore(subPath, value, subDictionary, createIfNotExist);
                    }
                    else
                    {
                        throw new InvalidOperationException("Undefined");
                    }
                }
                else
                {
                    dictionary[key] = value;
                }
            }
            else if (createIfNotExist)
            {
                if (segments.Length > 1)
                {
                    var subPath = string.Join(".", segments.Skip(1));
                    SetValueCore(subPath, value, new Dictionary<string, object>(), true);
                }
                else
                {
                    dictionary.Add(key, value);
                }
            }
            else
            {
                throw new InvalidOperationException("Undefined");
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
    }
}