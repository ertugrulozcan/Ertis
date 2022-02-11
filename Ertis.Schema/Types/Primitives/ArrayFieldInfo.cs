using System;
using System.Collections.Generic;
using System.Linq;
using Ertis.Schema.Dynamics;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Extensions;
using Ertis.Schema.Serialization;
using Ertis.Schema.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.Primitives
{
    public class ArrayFieldInfo : FieldInfo<Array>
    {
        #region Fields

        private readonly int? minCount;
        private readonly int? maxCount;
        private readonly IFieldInfo itemSchema;
        private readonly IEnumerable<string> uniqueBy;

        #endregion
        
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override FieldType Type => FieldType.array;
        
        [JsonProperty("itemSchema")]
        [JsonConverter(typeof(FieldInfoJsonConverter))]
        public IFieldInfo ItemSchema
        {
            get => this.itemSchema;
            init
            {
                this.itemSchema = value;
                if (value != null)
                {
                    value.Parent = this;
                }
                
                if (!this.ValidateItemSchema(out var exception))
                {
                    throw exception;
                }
                
                if (!this.ValidateUniqueBy(out var exception2))
                {
                    throw exception2;
                }
            }
        }

        [JsonProperty("minCount", NullValueHandling = NullValueHandling.Ignore)]
        public int? MinCount
        {
            get => this.minCount;
            init
            {
                this.minCount = value;
                
                if (!this.ValidateMinCount(out var exception))
                {
                    throw exception;
                }
            }
        }
        
        [JsonProperty("maxCount", NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxCount
        {
            get => this.maxCount;
            init
            {
                this.maxCount = value;
                
                if (!this.ValidateMaxCount(out var exception))
                {
                    throw exception;
                }
            }
        }

        [JsonProperty("uniqueItems", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool UniqueItems { get; init; }

        [JsonProperty("uniqueBy", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IEnumerable<string> UniqueBy
        {
            get => this.uniqueBy;
            init
            {
                this.uniqueBy = value;
                
                if (!this.ValidateUniqueBy(out var exception))
                {
                    throw exception;
                }
            }
        }

        #endregion

        #region Methods

        protected override void OnReady()
        {
            if (!this.ValidateUniqueBy(out var exception2))
            {
                throw exception2;
            }
        }

        public override bool ValidateSchema(out Exception exception)
        {
            base.ValidateSchema(out exception);
            this.ValidateItemSchema(out exception);
            this.ValidateMinCount(out exception);
            this.ValidateMaxCount(out exception);

            return exception == null;
        }

        public override bool Validate(object obj, IValidationContext validationContext)
        {
            var isValid = base.Validate(obj, validationContext);

            if (obj is Array array)
            {
                if (this.MaxCount != null && array.Length > this.MaxCount.Value)
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"Array length can not be greater than {this.MaxCount}", this));
                }
                
                if (this.MinCount != null && array.Length < this.MinCount.Value)
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"Array length can not be less than {this.MinCount}", this));
                }

                if (this.UniqueItems)
                {
                    var uniqueCount = array.Cast<object>().Distinct().Count();
                    if (array.Length != uniqueCount)
                    {
                        isValid = false;
                        validationContext.Errors.Add(new FieldValidationException("Array items must be unique", this));
                    }
                }

                // Item validations
                foreach (var item in array)
                {
                    var itemFieldInfo = (FieldInfo) this.ItemSchema.Clone();
                    isValid &= itemFieldInfo.Validate(item, validationContext);
                }
                
                // UniqueBy constraint validation
                if (this.UniqueBy != null)
                {
                    foreach (var uniqueByPath in this.UniqueBy)
                    {
                        var values = new List<object>();
                        foreach (var item in array)
                        {
                            var dynamicObject = new DynamicObject(item);
                            if (dynamicObject.TryGetValue(uniqueByPath, out var val, out _))
                            {
                                values.Add(val);
                            }
                        }

                        if (values.Count != values.Distinct().Count())
                        {
                            isValid = false;
                            validationContext.Errors.Add(new FieldValidationException($"Array items must be unique by the '{uniqueByPath}' field", this));
                        }
                    }
                }
            }
            
            return isValid;
        }

        private bool ValidateItemSchema(out Exception exception)
        {
            if (this.ItemSchema == null)
            {
                exception = new FieldValidationException($"Item schema is required for array ('{this.Name}')", this);
                return false;
            }

            this.ItemSchema.ValidateSchema(out exception);
            return exception == null;
        }
        
        private bool ValidateMinCount(out Exception exception)
        {
            if (this.MinCount != null)
            {
                if (this.MinCount < 0)
                {
                    exception = new FieldValidationException($"The 'minCount' value can not be less than zero ('{this.Name}')", this);
                    return false;
                }

                if (this.MaxCount != null && this.MinCount != null && this.MaxCount < this.MinCount)
                {
                    exception = new FieldValidationException($"The 'minCount' value can not be greater than the 'maxCount' value ('{this.Name}')", this);
                    return false;
                }
            }
            
            exception = null;
            return true;
        }
        
        private bool ValidateMaxCount(out Exception exception)
        {
            if (this.MaxCount != null)
            {
                if (this.MaxCount < 0)
                {
                    exception = new FieldValidationException($"The 'maxCount' value can not be less than zero ('{this.Name}')", this);
                    return false;
                }

                if (this.MinCount != null && this.MaxCount != null && this.MinCount > this.MaxCount)
                {
                    exception = new FieldValidationException($"The 'minCount' value can not be greater than the 'maxCount' value ('{this.Name}')", this);
                    return false;
                }
            }
            
            exception = null;
            return true;
        }
        
        private bool ValidateUniqueBy(out Exception exception)
        {
            if (this.ItemSchema != null && this.UniqueBy != null && this.Name != null)
            {
                if (this.UniqueBy.Any() && this.ItemSchema.Type != FieldType.@object)
                {
                    exception = new FieldValidationException($"The 'uniqueBy' feature only can be used with object-type items. Use the 'uniqueItems' feature for the primitive types. ('{this.Name}')", this);
                    return false;
                }

                if (this.ItemSchema is ISchema arrayItemSchema)
                {
                    foreach (var uniqueByPath in this.UniqueBy)
                    {
                        var childFieldInfo = arrayItemSchema.FindField($"{this.Path}[].{uniqueByPath}");
                        if (childFieldInfo == null)
                        {
                            exception = new FieldValidationException($"The '{this.Name}' array schema does not contains any field that has '{uniqueByPath}' path for the 'uniqueBy' constraint", this);
                            return false;
                        }
                        else if (childFieldInfo is not IPrimitiveType)
                        {
                            exception = new FieldValidationException($"The target field must be primitive type for the 'uniqueBy' constraint. ('{this.Name}')", this);
                            return false;
                        }
                    }   
                }
            }
            
            exception = null;
            return true;
        }

        internal int? IndexOf(FieldInfo itemFieldInfo)
        {
            if (itemFieldInfo is { CurrentObject: { } } && this.CurrentObject is Array array)
            {
                return Array.IndexOf(array, itemFieldInfo.CurrentObject);
            }

            return null;
        }

        public override object Clone()
        {
            return new ArrayFieldInfo
            {
                Name = this.Name,
                Description = this.Description,
                DisplayName = this.DisplayName,
                Parent = this.Parent,
                IsRequired = this.IsRequired,
                DefaultValue = this.DefaultValue,
                MinCount = this.MinCount,
                MaxCount = this.MaxCount,
                UniqueItems = this.UniqueItems,
                ItemSchema = this.ItemSchema.Clone() as IFieldInfo
            };
        }

        #endregion
    }
}