using System.Text.Json.Serialization;
using Ertis.Schema.Dynamics.Legacy;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Extensions;
using Ertis.Schema.Serialization;
using Ertis.Schema.Validation;

namespace Ertis.Schema.Types.Primitives;

public class ArrayFieldInfo : FieldInfo<Array>
{
    #region Properties
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Newtonsoft.Json.JsonProperty("type")]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public override FieldType Type => FieldType.array;
    
    [JsonPropertyName("itemSchema")]
    [Newtonsoft.Json.JsonProperty("itemSchema")]
    [Newtonsoft.Json.JsonConverter(typeof(FieldInfoJsonConverter))]
    public IFieldInfo? ItemSchema
    {
        get;
        init
        {
            field = value;
            if (value != null)
            {
                value.Parent = this;
            }
            
            if (!this.ValidateItemSchema(out var exception) && exception != null)
            {
                throw exception;
            }
            
            if (!this.ValidateUniqueBy(out var exception2) && exception2 != null)
            {
                throw exception2;
            }
        }
    }
    
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
    
    [JsonPropertyName("uniqueItems")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("uniqueItems", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    public bool UniqueItems { get; init; }
    
    [JsonPropertyName("uniqueBy")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("uniqueBy", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    public IEnumerable<string>? UniqueBy
    {
        get;
        init
        {
            field = value;
            
            if (!this.ValidateUniqueBy(out var exception) && exception != null)
            {
                throw exception;
            }
        }
    }
    
    #endregion
    
    #region Methods
    
    protected override void OnPropertyChanged(string propertyName)
    {
        if (propertyName == nameof(this.Name) && !this.ValidateUniqueBy(out var exception) && exception != null)
        {
            throw exception;
        }
    }
    
    public override bool ValidateSchema(out Exception? exception)
    {
        base.ValidateSchema(out exception);
        this.ValidateItemSchema(out exception);
        this.ValidateMinCount(out exception);
        this.ValidateMaxCount(out exception);
        
        return exception == null;
    }
    
    protected internal override bool Validate(object? obj, IValidationContext validationContext)
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
            if (this.ItemSchema != null)
            {
                var itemFieldInfo = (FieldInfo) this.ItemSchema.Clone();
                foreach (var item in array)
                {
                    isValid &= itemFieldInfo.Validate(item, validationContext);
                }
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
                        if (dynamicObject.TryGetValue(uniqueByPath, out var val, out _) && val != null)
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
    
    private bool ValidateItemSchema(out Exception? exception)
    {
        if (this.ItemSchema == null)
        {
            exception = new FieldValidationException($"Item schema is required for array ('{this.Name}')", this);
            return false;
        }
        
        this.ItemSchema.ValidateSchema(out exception);
        return exception == null;
    }
    
    private bool ValidateMinCount(out Exception? exception)
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
    
    private bool ValidateMaxCount(out Exception? exception)
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
    
    private bool ValidateUniqueBy(out Exception? exception)
    {
        if (this.ItemSchema != null && this.UniqueBy != null)
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
        if (itemFieldInfo is { CurrentObject: not null } && this.CurrentObject is Array array)
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
            IsVirtual = this.IsVirtual,
            IsHidden = this.IsHidden,
            IsReadonly = this.IsReadonly,
            IsSearchable = this.IsSearchable,
            SearchWeight = this.SearchWeight,
            DefaultValue = this.DefaultValue,
            MinCount = this.MinCount,
            MaxCount = this.MaxCount,
            UniqueItems = this.UniqueItems,
            ItemSchema = (IFieldInfo?)this.ItemSchema?.Clone(),
            UniqueBy = this.UniqueBy,
            Appearance = this.Appearance
        };
    }
    
    #endregion
}