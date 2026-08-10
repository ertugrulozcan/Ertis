using System.Text.Json.Serialization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Validation;

namespace Ertis.Schema.Types.Primitives;

public class EnumFieldInfo : FieldInfo<object>, IPrimitiveType
{
    #region Properties
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Newtonsoft.Json.JsonProperty("type")]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public override FieldType Type => FieldType.@enum;
    
    [JsonPropertyName("items")]
    [Newtonsoft.Json.JsonProperty("items")]
    public EnumItem[] Items
    {
        get => field ?? Array.Empty<EnumItem>();
        set
        {
            field = value;
            
            if (!this.ValidateItems(out var exception) && exception != null)
            {
                throw exception;
            }
        }
    }
    
    [JsonPropertyName("isUnique")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("isUnique", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    public bool IsUnique { get; set; }
    
    [JsonPropertyName("isMultiple")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("isMultiple", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    public bool IsMultiple { get; set; }
    
    #endregion
    
    #region Methods
    
    public override bool ValidateSchema(out Exception? exception)
    {
        base.ValidateSchema(out exception);
        this.ValidateItems(out exception);
        
        return exception == null;
    }
    
    protected internal override bool Validate(object? obj, IValidationContext validationContext)
    {
        var isValid = base.Validate(obj, validationContext);
        
        var isExistInEnums = false;
        if (obj != null)
        {
            if (this.IsMultiple)
            {
                if (obj is object[] array)
                {
                    isExistInEnums = array.All(item => this.Items.Any(x => x.Value.Equals(item)));
                }
                else
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"Enum value is must be array type ({this.Name})", this));   
                }
            }
            else
            {
                var type = obj.GetType();
                if (type.IsPrimitive || type == typeof(string))
                {
                    isExistInEnums = this.Items.Any(x => x.Value.Equals(obj));
                }
                else
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"Enum value is must be primitive type ({this.Name})", this));   
                }
            }
        }
        
        if (isValid && obj != null && !isExistInEnums)
        {
            isValid = false;
            var enumValues = string.Join(", ", this.Items.Select(x => $"'{x.Value}'"));
            validationContext.Errors.Add(new FieldValidationException($"The value does not exist in the enum items. The '{this.Name}' value must be one of them [{enumValues}]", this));   
        }
        
        return isValid;
    }
    
    private bool ValidateItems(out Exception? exception)
    {
        if (this.Items.Length == 0)
        {
            throw new FieldValidationException("Enum items can not be empty", this);
        }
        
        if (this.Items.Any(x => !x.Value.GetType().IsPrimitive && x.Value.GetType() != typeof(string)))
        {
            throw new FieldValidationException("Enum item values must be primitive type", this);
        }
        
        var uniqueCount = this.Items.Select(x => x.Value).Distinct().Count();
        if (this.Items.Length != uniqueCount)
        {
            throw new FieldValidationException("Enum items must be unique", this);
        }
        
        exception = null;
        return true;
    }
    
    public override object Clone()
    {
        return new EnumFieldInfo
        {
            Name = this.Name,
            Description = this.Description,
            DisplayName = this.DisplayName,
            Parent = this.Parent,
            IsRequired = this.IsRequired,
            IsUnique = this.IsUnique,
            IsVirtual = this.IsVirtual,
            IsHidden = this.IsHidden,
            IsReadonly = this.IsReadonly,
            DefaultValue = this.DefaultValue,
            Items = this.Items,
            IsMultiple = this.IsMultiple
        };
    }
    
    #endregion
    
    #region Helper Classes
    
    public class EnumItem
    {
        #region Properties
        
        [JsonPropertyName("displayName")]
        [Newtonsoft.Json.JsonProperty("displayName")]
        public required string DisplayName { get; set; }
        
        [JsonPropertyName("value")]
        [Newtonsoft.Json.JsonProperty("value")]
        public required string Value { get; set; }
        
        #endregion
    }
    
    #endregion
}