using System.Text.Json.Serialization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Helpers;
using Ertis.Schema.Validation;

namespace Ertis.Schema.Types.Primitives;

public class FloatFieldInfo : FieldInfo<double?>, IPrimitiveType
{
    #region Properties
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Newtonsoft.Json.JsonProperty("type")]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public override FieldType Type => FieldType.@float;
    
    /// <summary>
    /// Greater than or equal
    /// </summary>
    [JsonPropertyName("minimum")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty("minimum", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public double? Minimum
    {
        get;
        init
        {
            field = value;
            
            if (!this.ValidateMinimum(out var exception) && exception != null)
            {
                throw exception;
            }
        }
    }
    
    /// <summary>
    /// Less than or equal
    /// </summary>
    [JsonPropertyName("maximum")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty("maximum", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public double? Maximum
    {
        get;
        init
        {
            field = value;
            
            if (!this.ValidateMaximum(out var exception) && exception != null)
            {
                throw exception;
            }
        }
    }
    
    /// <summary>
    /// Greater than
    /// </summary>
    [JsonPropertyName("exclusiveMinimum")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty("exclusiveMinimum", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public double? ExclusiveMinimum
    {
        get;
        init
        {
            field = value;
            
            if (!this.ValidateExclusiveMinimum(out var exception) && exception != null)
            {
                throw exception;
            }
        }
    }
    
    /// <summary>
    /// Less than or equal
    /// </summary>
    [JsonPropertyName("exclusiveMaximum")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [Newtonsoft.Json.JsonProperty("exclusiveMaximum", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public double? ExclusiveMaximum
    {
        get;
        init
        {
            field = value;
            
            if (!this.ValidateExclusiveMaximum(out var exception) && exception != null)
            {
                throw exception;
            }
        }
    }
    
    [JsonPropertyName("isUnique")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("isUnique", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    public bool IsUnique { get; set; }
    
    #endregion
    
    #region Methods
    
    public override bool ValidateSchema(out Exception? exception)
    {
        base.ValidateSchema(out exception);
        this.ValidateMinimum(out exception);
        this.ValidateMaximum(out exception);
        this.ValidateExclusiveMinimum(out exception);
        this.ValidateExclusiveMaximum(out exception);
        
        return exception == null;
    }
    
    protected internal override bool Validate(object? obj, IValidationContext validationContext)
    {
        var isValid = base.Validate(obj, validationContext);
        
        if (obj != null)
        {
            var isCompatibleForDouble = NumericTypeHelper.IsAssignableTo(obj.GetType(), typeof(double));
            if (isCompatibleForDouble != null && isCompatibleForDouble.Value && double.TryParse(obj.ToString(), out var doubleValue))
            {
                if (this.Maximum != null && doubleValue > this.Maximum.Value)
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"The '{this.Name}' value can not be greater than {this.Maximum}", this));
                }
            
                if (this.Minimum != null && doubleValue < this.Minimum.Value)
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"The '{this.Name}' value can not be less than {this.Minimum}", this));
                }
            
                if (this.ExclusiveMaximum != null && doubleValue >= this.ExclusiveMaximum.Value)
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"The '{this.Name}' value can not be greater than or equal {this.ExclusiveMaximum}", this));
                }
            
                if (this.ExclusiveMinimum != null && doubleValue < this.ExclusiveMinimum.Value)
                {
                    isValid = false;
                    validationContext.Errors.Add(new FieldValidationException($"The '{this.Name}' value can not be less than or equal {this.ExclusiveMinimum}", this));
                }
            }
        }
        
        return isValid;
    }
    
    private bool ValidateMinimum(out Exception? exception)
    {
        if (this.Maximum != null && this.Minimum != null && this.Maximum < this.Minimum)
        {
            exception = new FieldValidationException($"The 'minimum' value can not be greater than the 'maximum' value ({this.Name})", this);
            return false;
        }
        
        if (this.ExclusiveMaximum != null && this.Minimum != null && this.ExclusiveMaximum < this.Minimum)
        {
            exception = new FieldValidationException($"The 'minimum' value can not be greater than the 'exclusiveMaximum' value ({this.Name})", this);
            return false;
        }
        
        exception = null;
        return true;
    }
    
    private bool ValidateMaximum(out Exception? exception)
    {
        if (this.Minimum != null && this.Maximum != null && this.Minimum > this.Maximum)
        {
            exception = new FieldValidationException($"The 'maximum' value can not be less than the 'minimum' value ({this.Name})", this);
            return false;
        }
        
        if (this.ExclusiveMinimum != null && this.Maximum != null && this.ExclusiveMinimum > this.Maximum)
        {
            exception = new FieldValidationException($"The 'maximum' value can not be less than the 'exclusiveMinimum' value ({this.Name})", this);
            return false;
        }
        
        exception = null;
        return true;
    }
    
    private bool ValidateExclusiveMinimum(out Exception? exception)
    {
        if (this.Maximum != null && this.ExclusiveMinimum != null && this.Maximum < this.ExclusiveMinimum)
        {
            exception = new FieldValidationException($"The 'exclusiveMinimum' value can not be greater than the 'maximum' value ({this.Name})", this);
            return false;
        }
        
        if (this.ExclusiveMaximum != null && this.ExclusiveMinimum != null && this.ExclusiveMaximum < this.ExclusiveMinimum)
        {
            exception = new FieldValidationException($"The 'exclusiveMinimum' value can not be greater than the 'exclusiveMaximum' value ({this.Name})", this);
            return false;
        }
        
        exception = null;
        return true;
    }
    
    private bool ValidateExclusiveMaximum(out Exception? exception)
    {
        if (this.Minimum != null && this.ExclusiveMaximum != null && this.Minimum > this.ExclusiveMaximum)
        {
            exception = new FieldValidationException($"The 'exclusiveMaximum' value can not be less than the 'minimum' value ({this.Name})", this);
            return false;
        }
        
        if (this.ExclusiveMinimum != null && this.ExclusiveMaximum != null && this.ExclusiveMinimum > this.ExclusiveMaximum)
        {
            exception = new FieldValidationException($"The 'exclusiveMaximum' value can not be less than the 'exclusiveMinimum' value ({this.Name})", this);
            return false;
        }
        
        exception = null;
        return true;
    }
    
    public override object Clone()
    {
        return new FloatFieldInfo
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
            Minimum = this.Minimum,
            Maximum = this.Maximum,
            ExclusiveMinimum = this.ExclusiveMinimum,
            ExclusiveMaximum = this.ExclusiveMaximum
        };
    }
    
    #endregion
}