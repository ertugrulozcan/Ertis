using System.Text.Json.Serialization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Validation;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Ertis.Schema.Types.Primitives;

public class ConstantFieldInfo : FieldInfo<object>
{
    #region Properties
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public override FieldType Type => FieldType.@const;
    
    [JsonPropertyName("value")]
    public object? Value { get; set; }
    
    [JsonPropertyName("valueType")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ConstantType ValueType { get; set; }
    
    #endregion
    
    #region Methods
    
    protected internal override bool Validate(object? obj, IValidationContext validationContext)
    {
        var isValid = base.Validate(obj, validationContext);
        
        if (obj != null)
        {
            var type = obj.GetType();
            var incompatibleType = this.ValueType switch
            {
                ConstantType.@string => type != typeof(string),
                ConstantType.integer => !int.TryParse(obj.ToString(), out _),
                ConstantType.@float => !double.TryParse(obj.ToString(), out _),
                ConstantType.boolean => !bool.TryParse(obj.ToString(), out _),
                ConstantType.date or ConstantType.datetime => !DateTime.TryParse(obj.ToString(), out _),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            if (incompatibleType)
            {
                isValid = false;
                validationContext.Errors.Add(new FieldValidationException($"Constant value is must be {this.ValueType}", this));
            }
        }
        
        return isValid;
    }
    
    public override object Clone()
    {
        return new ConstantFieldInfo
        {
            Name = this.Name,
            Description = this.Description,
            DisplayName = this.DisplayName,
            Parent = this.Parent,
            IsRequired = this.IsRequired,
            IsVirtual = this.IsVirtual,
            IsHidden = this.IsHidden,
            IsReadonly = this.IsReadonly,
            DefaultValue = this.DefaultValue,
            Value = this.Value,
            ValueType = this.ValueType
        };
    }
    
    #endregion
    
    #region Enums
    
    public enum ConstantType
    {
        @string,
        integer,
        @float,
        boolean,
        date,
        datetime
    }
    
    #endregion
}