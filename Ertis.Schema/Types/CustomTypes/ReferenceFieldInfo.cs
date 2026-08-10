using System.Text.Json.Serialization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Models;
using Ertis.Schema.Validation;

// ReSharper disable UnusedMember.Global
namespace Ertis.Schema.Types.CustomTypes;

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
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Newtonsoft.Json.JsonProperty("type")]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public override FieldType Type => FieldType.reference;
    
    [JsonPropertyName("referenceType")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Newtonsoft.Json.JsonProperty("referenceType")]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public ReferenceTypes ReferenceType { get; set; }
    
    [JsonPropertyName("contentType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("contentType", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    public string? ContentType { get; set; }
    
    [JsonPropertyName("singleReferenceOptions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("singleReferenceOptions", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    public SingleReferenceOptions? SingleReferenceOptions { get; set; }
    
    [JsonPropertyName("multipleReferenceOptions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("multipleReferenceOptions", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    public MultipleReferenceOptions? MultipleReferenceOptions { get; set; }
    
    [JsonPropertyName("collectionReferenceOptions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("collectionReferenceOptions", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    public CollectionReferenceOptions? CollectionReferenceOptions { get; set; }
    
    #endregion
    
    #region Methods
    
    protected override void ValidateSchemaCore(out Exception? exception)
    {
        exception = null;
    }
    
    protected override bool ValidateCore(object? obj, IValidationContext validationContext)
    {
        var isValid = true;
        
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
            // NOP
        }
        else
        {
            isValid = false;
            validationContext.Errors.Add(new FieldValidationException($"Invalid reference value on '{this.Name}' field", this));
        }
        
        return isValid;
    }
    
    private static string? EnsureReferenceId(object? obj)
    {
        return obj switch
        {
            string referenceId => referenceId,
            Dictionary<string, object> objectDictionary when objectDictionary.ContainsKey("_id") => objectDictionary["_id"].ToString(),
            _ => null
        };
    }
    
    public override object? GetDefaultValue()
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
            ReferenceType = this.ReferenceType
        };
    }
    
    #endregion
}

public class SingleReferenceOptions;

public class MultipleReferenceOptions
{
    #region Properties
    
    [JsonPropertyName("minCount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty("minCount", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? MinCount
    {
        get;
        init
        {
            field = value;
            
            if (!this.ValidateMinCount(out var exception) && exception != null)
            {
                throw exception;
            }
        }
    }
    
    [JsonPropertyName("maxCount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty("maxCount", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? MaxCount
    {
        get;
        init
        {
            field = value;
            
            if (!this.ValidateMaxCount(out var exception) && exception != null)
            {
                throw exception;
            }
        }
    }
    
    #endregion
    
    #region Methods
    
    private bool ValidateMinCount(out Exception? exception)
    {
        if (this.MinCount != null)
        {
            if (this.MinCount < 0)
            {
                exception = new SchemaValidationException("The multiple reference 'minCount' value can not be less than zero");
                return false;
            }
            
            if (this.MaxCount != null && this.MinCount != null && this.MaxCount < this.MinCount)
            {
                exception = new SchemaValidationException("The multiple reference 'minCount' value can not be greater than the 'maxCount' value");
                return false;
            }
        }
        
        exception = null;
        return true;
    }
    
    private bool ValidateMaxCount(out Exception? exception)
    {
        if (this.MaxCount != null)
        {
            if (this.MaxCount < 0)
            {
                exception = new SchemaValidationException("The multiple reference 'maxCount' value can not be less than zero");
                return false;
            }
            
            if (this.MinCount != null && this.MaxCount != null && this.MinCount > this.MaxCount)
            {
                exception = new SchemaValidationException("The multiple reference 'minCount' value can not be greater than the 'maxCount' value");
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
    #region Properties
    
    [JsonPropertyName("collection")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty("collection", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string? CollectionSlug { get; set; }
    
    [JsonPropertyName("skip")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty("skip", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? Skip
    {
        get;
        init
        {
            field = value;
            
            if (!this.ValidateSkipValue(out var exception) && exception != null)
            {
                throw exception;
            }
        }
    }
    
    [JsonPropertyName("limit")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty("limit", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? Limit
    {
        get;
        init
        {
            field = value;
            
            if (!this.ValidateLimitValue(out var exception) && exception != null)
            {
                throw exception;
            }
        }
    }
    
    [JsonPropertyName("asObject")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty("asObject", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public bool? AsObject { get; set; }
    
    [JsonPropertyName("queryParams")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty("queryParams", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public CollectionReferenceParameter[]? QueryParams { get; set; }
    
    [JsonPropertyName("excludedFields")]
    [Newtonsoft.Json.JsonProperty("excludedFields")]
    public string[]? ExcludedFields { get; set; }
    
    #endregion
    
    #region Methods
    
    private bool ValidateSkipValue(out Exception? exception)
    {
        if (this.Skip != null)
        {
            switch (this.Skip)
            {
                case < 0:
                    exception = new SchemaValidationException("The multiple reference 'skip' value can not be less than zero");
                    return false;
                case > 500:
                    exception = new SchemaValidationException("The multiple reference 'skip' value can not be greater than 500");
                    return false;
            }
        }
        
        exception = null;
        return true;
    }
    
    private bool ValidateLimitValue(out Exception? exception)
    {
        if (this.Limit != null)
        {
            switch (this.Limit)
            {
                case <= 0:
                    exception = new SchemaValidationException("The multiple reference 'limit' value can not be less than or equal zero");
                    return false;
                case > 500:
                    exception = new SchemaValidationException("The multiple reference 'limit' value can not be greater than 500");
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
    
    [JsonPropertyName("value")]
    [Newtonsoft.Json.JsonProperty("value")]
    public object? Value { get; set; }
    
    [JsonPropertyName("bindingType")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Newtonsoft.Json.JsonProperty("bindingType")]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public BindingTypes BindingType { get; set; }
    
    [JsonPropertyName("dynamicParameter")]
    [Newtonsoft.Json.JsonProperty("dynamicParameter")]
    public string? DynamicParameter { get; set; }
	
    #endregion
}

public enum BindingTypes
{
    @static,
    dynamic
}