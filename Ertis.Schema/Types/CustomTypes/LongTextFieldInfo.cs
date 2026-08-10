using System.Text.Json.Serialization;
using Ertis.Schema.Types.Primitives;

namespace Ertis.Schema.Types.CustomTypes;

public class LongTextFieldInfo : StringFieldInfo
{
    #region Properties
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Newtonsoft.Json.JsonProperty("type")]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public override FieldType Type => FieldType.longtext;
    
    #endregion
    
    #region Methods
    
    public override object Clone()
    {
        return new LongTextFieldInfo
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
            MinLength = this.MinLength,
            MaxLength = this.MaxLength,
            FormatPattern = this.FormatPattern,
            RegexPattern = this.RegexPattern,
            RestrictRegexPattern = this.RestrictRegexPattern,
            CaseInsensitive = this.CaseInsensitive
        };
    }
    
    #endregion
}