using System.Text.Json.Serialization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Validation;

// ReSharper disable UnusedMethodReturnValue.Local
namespace Ertis.Schema.Types;

public abstract class FieldInfo : IFieldInfo, IHasDefault
{
    #region Properties
    
    [JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public required string Name
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged(nameof(this.Name));
        }
    }
    
    [JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public IFieldInfo? Parent
    {
        get;
        set
        {
            field = value;
            this.OnPropertyChanged(nameof(this.Parent));
        }
    }
    
    [JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public string Path
    {
        get
        {
            var path = this.Parent != null ? $"{this.Parent.Path}.{this.Name}" : this.Name;
            if (this.Parent is ArrayFieldInfo arrayFieldInfo)
            {
                path = $"{this.Parent.Path}[{arrayFieldInfo.IndexOf(this)}]";   
            }
            
            return path;
        }
    }
    
    [JsonPropertyName("displayName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty("displayName", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string DisplayName
    {
        get => field ?? this.Name;
        set
        {
            field = value;
            this.OnPropertyChanged(nameof(this.DisplayName));
        }
    }
    
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("description", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    public string? Description
    {
        get;
        init
        {
            field = value;
            this.OnPropertyChanged(nameof(this.Description));
        }
    }
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Newtonsoft.Json.JsonProperty("type")]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public abstract FieldType Type { get; }
    
    [JsonPropertyName("isRequired")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("isRequired", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    public bool IsRequired
    {
        get;
        init
        {
            field = value;
            this.OnPropertyChanged(nameof(this.IsRequired));
        }
    }
    
    [JsonPropertyName("isVirtual")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("isVirtual", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    public bool IsVirtual
    {
        get;
        init
        {
            field = value;
            this.OnPropertyChanged(nameof(this.IsVirtual));
        }
    }
    
    [JsonPropertyName("isHidden")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("isHidden", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    public bool IsHidden
    {
        get;
        init
        {
            field = value;
            this.OnPropertyChanged(nameof(this.IsHidden));
        }
    }
    
    [JsonPropertyName("isReadonly")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("isReadonly", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    public bool IsReadonly
    {
        get;
        init
        {
            field = value;
            this.OnPropertyChanged(nameof(this.IsReadonly));
        }
    }
    
    [JsonPropertyName("appearance")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("appearance", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    public string? Appearance
    {
        get;
        init
        {
            field = value;
            this.OnPropertyChanged(nameof(this.Appearance));
        }
    }
    
    [JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    protected internal object? CurrentObject { get; private set; }
    
    [JsonPropertyName("isSearchable")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("isSearchable", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    public bool IsSearchable
    {
        get;
        init
        {
            field = value;
            this.OnPropertyChanged(nameof(this.IsSearchable));
        }
    }
    
    [JsonPropertyName("searchWeight")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("searchWeight", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    public double? SearchWeight
    {
        get;
        init
        {
            field = value;
            this.OnPropertyChanged(nameof(this.SearchWeight));
        }
    }
    
    #endregion
    
    #region Abstract Methods
    
    protected abstract void ValidateSchemaCore(out Exception? exception);
    
    protected abstract bool ValidateCore(object? obj, IValidationContext validationContext);
    
    public abstract object? GetDefaultValue();
    
    public abstract object Clone();
    
    #endregion
    
    #region Methods
    
    protected virtual void OnPropertyChanged(string propertyName)
    {
        // NOP
    }
    
    public virtual bool ValidateSchema(out Exception? exception)
    {
        this.ValidateName(out exception);
        this.ValidateSchemaCore(out exception);
        
        return exception == null;
    }
    
    protected internal virtual bool Validate(object? obj, IValidationContext validationContext)
    {
        this.CurrentObject = obj;
        
        var isValid = true;
        if (validationContext == null)
        {
            throw new ArgumentNullException(nameof(validationContext), "ValidationContext is null");
        }
        
        switch (obj)
        {
            case null when !this.IsRequired:
                break;
            case null when this.IsRequired && this.GetDefaultValue() == null:
                isValid = false;
                validationContext.Errors.Add(new FieldValidationException($"{this.Name} is required", this));
                break;
            default:
            {
                isValid &= ValidateCore(obj, validationContext);
                break;
            }
        }
        
        return isValid;
    }
    
    private bool ValidateName(out Exception? exception)
    {
        if (string.IsNullOrEmpty(this.Name.Trim()) || string.IsNullOrWhiteSpace(this.Name))
        {
            exception = new FieldValidationException("The field name is required", this);
            return false;
        }
        
        if (this.Name.Contains(' '))
        {
            exception = new FieldValidationException("The field name can not include any whitespace", this);
            return false;
        }
        
        if (char.IsDigit(this.Name[0]))
        {
            exception = new FieldValidationException("The field name can not start with a digit", this);
            return false;
        }
        
        if (!this.Name.Where(x => x != '_').All(char.IsLetterOrDigit))
        {
            exception = new FieldValidationException("The field names can only use letters, digits and underscore", this);
            return false;
        }
        
        exception = null;
        return true;
    }
    
    public bool IsAnArrayItem(out ArrayFieldInfo? arrayFieldInfo)
    {
        switch (this.Parent)
        {
            case ArrayFieldInfo parentArrayFieldInfo:
                arrayFieldInfo = parentArrayFieldInfo;
                return true;
            case FieldInfo parentFieldInfo:
                return parentFieldInfo.IsAnArrayItem(out arrayFieldInfo);
            default:
                arrayFieldInfo = null;
                return false;
        }
    }
    
    #endregion
}

public abstract class FieldInfo<T> : FieldInfo, IHasDefault<T>
{
    #region Properties
    
    [JsonPropertyName("defaultValue")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("defaultValue", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    public T? DefaultValue
    {
        get;
        init
        {
            field = value;
            this.OnPropertyChanged(nameof(this.DefaultValue));
        }
    }
    
    #endregion
    
    #region Methods
    
    protected override bool ValidateCore(object? obj, IValidationContext validationContext)
    {
        var isValid = true;
        
        var isCompatibleType = this.IsCompatibleType(obj);
        if (obj != null && !isCompatibleType)
        {
            if (string.IsNullOrEmpty(this.Name) && this.Parent is {Type: FieldType.array})
            {
                isValid = false;
                validationContext.Errors.Add(new FieldValidationException($"Type mismatch error. Array items are must be '{GetPrimitiveName(typeof(T))}'", this));
            }
            else
            {
                isValid = false;
                validationContext.Errors.Add(new FieldValidationException($"Type mismatch error. '{this.Name}' is must be '{GetPrimitiveName(typeof(T))}'", this));
            }
        }
        else if (this.Type == FieldType.@object && obj is not IDictionary<string, object>)
        {
            isValid = false;
            validationContext.Errors.Add(new FieldValidationException($"Type mismatch error. '{this.Name}' is must be 'object'", this));
        }
        
        return isValid;
    }
    
    protected override void ValidateSchemaCore(out Exception? exception)
    {
        this.ValidateDefaultValue(out exception);
    }
    
    protected override void OnPropertyChanged(string propertyName)
    {
        if (propertyName is nameof(this.IsRequired) or nameof(this.IsHidden) or nameof(this.IsReadonly) or nameof(this.DefaultValue))
        {
            this.ValidateHiddenRules();
        }
    }
    
    public override object? GetDefaultValue()
    {
        return this.DefaultValue;
    }
    
    private bool ValidateDefaultValue(out Exception? exception)
    {
        if (this.DefaultValue != null)
        {
            var validationContext = new FieldValidationContext(null);
            this.Validate(this.DefaultValue, validationContext);
            var isValidated = !validationContext.Errors.Any();
            exception = !isValidated ? validationContext.Errors.First() : null;
            
            return isValidated;    
        }
        
        exception = null;
        return true;
    }
    
    private bool IsCompatibleType(object? obj)
    {
        if (obj == null)
        {
            return false;
        }
        
        if (obj is not T)
        {
            if (this.Type is FieldType.date or FieldType.datetime && obj is DateTime && typeof(T) == typeof(string))
            {
                return true;
            }
            
            if (this.Type is FieldType.array or FieldType.tags && obj is object[] objectArray && (objectArray.Length == 0 || objectArray.All(x => x is string)))
            {
                // If array is empty, it's handled as an object array, it's valid.
                return true;
            }
            
            var isAssignableTo = Helpers.NumericTypeHelper.IsAssignableTo(obj.GetType(), typeof(T));
            return isAssignableTo != null && isAssignableTo.Value;
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
    
    protected virtual void ValidateHiddenRules()
    {
        if (this.IsHidden && this.IsRequired && this.DefaultValue == null)
        {
            throw new FieldValidationException("A field with a default value of null cannot be both hidden and required.", this);
        }
    }
    
    #endregion
}