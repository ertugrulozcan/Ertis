using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Models;
using Ertis.Schema.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes
{
    public class ReferenceFieldInfo : FieldInfo
    {
        #region Enums

        public enum ReferenceTypes
        {
            single,
            multiple,
            collection
        }

        #endregion

        #region Properties

        [JsonProperty("type")]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        [JsonPropertyName("type")]
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
        public override FieldType Type => FieldType.reference;
        
        [JsonProperty("referenceType")]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        [JsonPropertyName("referenceType")]
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
        public ReferenceTypes ReferenceType { get; set; }
        
        [JsonProperty("contentType", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("contentType")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string ContentType { get; set; }
        
        [JsonProperty("singleReferenceOptions", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("singleReferenceOptions")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public SingleReferenceOptions SingleReferenceOptions { get; set; }
        
        [JsonProperty("multipleReferenceOptions", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("multipleReferenceOptions")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public MultipleReferenceOptions MultipleReferenceOptions { get; set; }
        
        [JsonProperty("collectionReferenceOptions", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("collectionReferenceOptions")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public CollectionReferenceOptions CollectionReferenceOptions { get; set; }

        #endregion

        #region Methods

        protected override void ValidateSchemaCore(out Exception exception)
        {
            exception = null;
        }

        protected override bool ValidateCore(object obj, IValidationContext validationContext)
        {
            var isValid = true;
            
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (this.ReferenceType == ReferenceTypes.single)
            {
                var hasReferenceId = EnsureReferenceId(obj) != null;
                isValid = hasReferenceId;
                if (!hasReferenceId)
                {
                    validationContext.Errors.Add(new FieldValidationException($"The reference field [{this.Name}] has no _id field", this));
                }
            }
            else if (this.ReferenceType == ReferenceTypes.multiple && obj is object[] objectArray)
            {
                isValid = objectArray.All(x => EnsureReferenceId(x) != null);
                if (this.MultipleReferenceOptions != null)
                {
                    if (this.MultipleReferenceOptions.MaxCount != null && objectArray.Length > this.MultipleReferenceOptions.MaxCount.Value)
                    {
                        isValid = false;
                        validationContext.Errors.Add(new FieldValidationException($"Multiple reference array length can not be greater than {this.MultipleReferenceOptions.MaxCount}", this));
                    }
                
                    if (this.MultipleReferenceOptions.MinCount != null && objectArray.Length < this.MultipleReferenceOptions.MinCount.Value)
                    {
                        isValid = false;
                        validationContext.Errors.Add(new FieldValidationException($"Multiple reference array length can not be less than {this.MultipleReferenceOptions.MinCount}", this));
                    }
                }
            }
            else if (this.ReferenceType == ReferenceTypes.collection)
            {
                
            }
            else
            {
                isValid = false;
                validationContext.Errors.Add(new FieldValidationException($"Invalid reference value on '{this.Name}' field", this));
            }
            
            return isValid;
        }

        private static string EnsureReferenceId(object obj)
        {
            return obj switch
            {
                string referenceId => referenceId,
                Dictionary<string, object> objectDictionary when objectDictionary.ContainsKey("_id") =>
                    objectDictionary["_id"].ToString(),
                _ => null
            };
        }
        
        public override object GetDefaultValue()
        {
            return null;
        }

        public override object Clone()
        {
            return new ReferenceFieldInfo
            {
                Name = this.Name,
                Description = this.Description,
                DisplayName = this.DisplayName,
                Parent = this.Parent,
                ContentType = this.ContentType,
                IsRequired = this.IsRequired,
                IsVirtual = this.IsVirtual,
                IsHidden = this.IsHidden,
                IsReadonly = this.IsReadonly,
                ReferenceType = this.ReferenceType,
            };
        }

        #endregion
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class SingleReferenceOptions
    {
        #region Properties

        

        #endregion
    }
    
    public class MultipleReferenceOptions
    {
        #region Fields

        private readonly int? minCount;
        private readonly int? maxCount;

        #endregion
        
        #region Properties

        [JsonProperty("minCount", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("minCount")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
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
        [JsonPropertyName("maxCount")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
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

        #endregion

        #region Methods

        private bool ValidateMinCount(out Exception exception)
        {
            if (this.MinCount != null)
            {
                if (this.MinCount < 0)
                {
                    exception = new SchemaValidationException($"The multiple reference 'minCount' value can not be less than zero");
                    return false;
                }

                if (this.MaxCount != null && this.MinCount != null && this.MaxCount < this.MinCount)
                {
                    exception = new SchemaValidationException($"The multiple reference 'minCount' value can not be greater than the 'maxCount' value");
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
                    exception = new SchemaValidationException($"The multiple reference 'maxCount' value can not be less than zero");
                    return false;
                }

                if (this.MinCount != null && this.MaxCount != null && this.MinCount > this.MaxCount)
                {
                    exception = new SchemaValidationException($"The multiple reference 'minCount' value can not be greater than the 'maxCount' value");
                    return false;
                }
            }
            
            exception = null;
            return true;
        }

        #endregion
    }
    
    public class CollectionReferenceOptions
    {
        #region Fields

        private readonly int? skip;
        private readonly int? limit;

        #endregion
        
        #region Properties

        [JsonProperty("collection", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("collection")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string CollectionSlug { get; set; }
        
        [JsonProperty("skip", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("skip")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Skip
        {
            get => this.skip;
            init
            {
                this.skip = value;
                
                if (!this.ValidateSkipValue(out var exception))
                {
                    throw exception;
                }
            }
        }
        
        [JsonProperty("limit", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("limit")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Limit
        {
            get => this.limit;
            init
            {
                this.limit = value;
                
                if (!this.ValidateLimitValue(out var exception))
                {
                    throw exception;
                }
            }
        }
        
        [JsonProperty("asObject", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("asObject")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? AsObject { get; set; }
        
        [JsonProperty("queryParams", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("queryParams")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CollectionReferenceParameter[] QueryParams { get; set; }
        
        [JsonProperty("excludedFields")]
        [JsonPropertyName("excludedFields")]
        public string[] ExcludedFields { get; set; }
        
        #endregion

        #region Methods

        private bool ValidateSkipValue(out Exception exception)
        {
            if (this.Skip != null)
            {
                switch (this.Skip)
                {
                    case < 0:
                        exception = new SchemaValidationException($"The multiple reference 'skip' value can not be less than zero");
                        return false;
                    case > 500:
                        exception = new SchemaValidationException($"The multiple reference 'skip' value can not be greater than 500");
                        return false;
                }
            }
            
            exception = null;
            return true;
        }
        
        private bool ValidateLimitValue(out Exception exception)
        {
            if (this.Limit != null)
            {
                switch (this.Limit)
                {
                    case <= 0:
                        exception = new SchemaValidationException($"The multiple reference 'limit' value can not be less than or equal zero");
                        return false;
                    case > 500:
                        exception = new SchemaValidationException($"The multiple reference 'limit' value can not be greater than 500");
                        return false;
                }
            }
            
            exception = null;
            return true;
        }

        #endregion
    }

    public class CollectionReferenceParameter : DynamicQueryParameter
    {
        #region Properties
		
        [JsonProperty("value")]
        [JsonPropertyName("value")]
        public object Value { get; set; }
		
        [JsonProperty("bindingType")]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        [JsonPropertyName("bindingType")]
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
        public BindingTypes BindingType { get; set; }
		
        [JsonProperty("dynamicParameter")]
        [JsonPropertyName("dynamicParameter")]
        public string DynamicParameter { get; set; }
		
        #endregion
    }

    public enum BindingTypes
    {
        @static,
        @dynamic
    }
}