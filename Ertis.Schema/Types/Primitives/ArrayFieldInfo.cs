using System;
using System.Linq;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Serialization;
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

        #endregion
        
        #region Properties

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override PrimitiveType Type => PrimitiveType.array;
        
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

        #endregion

        #region Methods

        public override bool ValidateSchema(out Exception exception)
        {
            base.ValidateSchema(out exception);
            this.ValidateItemSchema(out exception);
            this.ValidateMinCount(out exception);
            this.ValidateMaxCount(out exception);

            return exception == null;
        }

        protected override void Validate(object obj)
        {
            base.Validate(obj);

            if (obj is Array array)
            {
                var i = 0;
                foreach (var item in array)
                {
                    if (!this.ItemSchema.IsValid(item, out var ex))
                    {
                        throw new FieldValidationException($"The array contains some invalid items. ({ex.Message})", this, $"{this.Path}[{i}]");
                    }

                    i++;
                }
                
                if (this.MaxCount != null && array.Length > this.MaxCount.Value)
                {
                    throw new FieldValidationException($"Array length can not be greater than {this.MaxCount}", this);
                }
                
                if (this.MinCount != null && array.Length < this.MinCount.Value)
                {
                    throw new FieldValidationException($"Array length can not be less than {this.MinCount}", this);
                }

                if (this.UniqueItems)
                {
                    var uniqueCount = array.Cast<object>().Distinct().Count();
                    if (array.Length != uniqueCount)
                    {
                        throw new FieldValidationException("Array items must be unique", this);
                    }
                }
            }
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
        
        #endregion
    }
}