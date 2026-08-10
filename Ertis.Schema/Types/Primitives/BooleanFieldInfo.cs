using System.Text.Json.Serialization;

namespace Ertis.Schema.Types.Primitives;

public class BooleanFieldInfo : FieldInfo<bool?>, IPrimitiveType
{
    #region Properties
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Newtonsoft.Json.JsonProperty("type")]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public override FieldType Type => FieldType.boolean;
    
    [JsonPropertyName("isUnique")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [Newtonsoft.Json.JsonProperty("isUnique", NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore)]
    public bool IsUnique { get; set; }
    
    #endregion
    
    #region Methods
    
    public override bool ValidateSchema(out Exception? exception)
    {
        base.ValidateSchema(out exception);
        return exception == null;
    }
    
    public override object Clone()
    {
        return new BooleanFieldInfo
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
            DefaultValue = this.DefaultValue
        };
    }
    
    #endregion
}