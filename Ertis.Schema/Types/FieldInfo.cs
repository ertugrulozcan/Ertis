using System;
using System.Collections.Generic;
using System.Linq;
using Ertis.Schema.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types
{
    public abstract class FieldInfo<T> : IFieldInfo, IHasDefault<T>
    {
        #region Fields

        private readonly T defaultValue;

        #endregion

        #region Properties
        
        [JsonIgnore]
        public string Name { get; set; }
        
        [JsonIgnore]
        public IFieldInfo Parent { get; set; }

        [JsonIgnore]
        public string Path => this.Parent != null ? $"{this.Parent.Path}.{this.Name}" : this.Name;

        [JsonProperty("displayName", NullValueHandling = NullValueHandling.Ignore)]
        public string DisplayName { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Description { get; init; }

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public abstract PrimitiveType Type { get; }

        [JsonProperty("isRequired", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsRequired { get; init; }

        [JsonProperty("defaultValue", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public T DefaultValue
        {
            get => this.defaultValue;
            init
            {
                this.Validate(value);
                this.defaultValue = value;
            }
        }

        #endregion

        #region Abstract Methods

        public virtual bool ValidateSchema(out Exception exception)
        {
            this.ValidateName(out exception);
            
            return exception == null;
        }

        #endregion

        #region Methods

        protected virtual void Validate(object obj)
        {
            if (obj == null && this.IsRequired)
            {
                throw new FieldValidationException($"'{this.Name}' is required", this);
            }
            else if (obj != null && !IsCompatibleType(obj))
            {
                throw new FieldValidationException($"Type mismatch error. '{this.Name}' is must be '{GetPrimitiveName(typeof(T))}'", this);
            }
            else if (typeof(T) == typeof(object))
            {
                if (obj is not IDictionary<string, object>)
                {
                    throw new FieldValidationException($"Type mismatch error. '{this.Name}' is must be 'object'", this);
                }
            }
        }

        private static bool IsCompatibleType(object obj)
        {
            if (obj is not T)
            {
                var integerCompatibleTypes = new[]
                {
                    typeof(int),
                    typeof(uint),
                    typeof(nint),
                    typeof(nuint),
                    typeof(byte),
                    typeof(sbyte),
                    typeof(decimal),
                    typeof(long),
                    typeof(ulong),
                    typeof(short),
                    typeof(ushort)
                };
                
                var floatCompatibleTypes = new[]
                {
                    typeof(float),
                    typeof(double)
                };

                return integerCompatibleTypes.Contains(obj.GetType()) && 
                       integerCompatibleTypes.Contains(typeof(T)) ||
                       floatCompatibleTypes.Contains(obj.GetType()) && 
                       floatCompatibleTypes.Contains(typeof(T));
            }

            return true;
        }

        private static string GetPrimitiveName(Type type)
        {
            var primitiveTypeName = type.Name;
            
            if (type == typeof(int))
            {
                primitiveTypeName = "integer";
            }
            else if (type == typeof(double))
            {
                primitiveTypeName = "float";
            }
            else if (type == typeof(bool))
            {
                primitiveTypeName = "boolean";
            }
            else if (type == typeof(Array))
            {
                primitiveTypeName = "array";
            }
            else if (type == typeof(string))
            {
                primitiveTypeName = "string";
            }

            return primitiveTypeName;
        }

        public bool IsValid(object obj, out Exception exception)
        {
            try
            {
                this.Validate(obj);
                exception = null;
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }
        
        // ReSharper disable once UnusedMethodReturnValue.Local
        private bool ValidateName(out Exception exception)
        {
            if (this.Name == null || string.IsNullOrEmpty(this.Name.Trim()) || string.IsNullOrWhiteSpace(this.Name))
            {
                exception = new ErtisSchemaValidationException("The field name is required");
                return false;
            }

            if (this.Name.Contains(' '))
            {
                exception = new ErtisSchemaValidationException("The field name can not include any whitespace");
                return false;
            }
                
            if (char.IsDigit(this.Name[0]))
            {
                exception = new ErtisSchemaValidationException("The field name can not start with a digit");
                return false;
            }
                
            if (!this.Name.Where(x => x != '_').All(char.IsLetterOrDigit))
            {
                exception = new ErtisSchemaValidationException("The field names can only use letters, digits and underscore");
                return false;
            }
            
            exception = null;
            return true;
        }

        #endregion
    }
}