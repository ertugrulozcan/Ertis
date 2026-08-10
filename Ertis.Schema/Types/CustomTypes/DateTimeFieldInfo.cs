using System.Text.Json.Serialization;

namespace Ertis.Schema.Types.CustomTypes;

public class DateTimeFieldInfo : DateTimeFieldInfoBase
{
    #region Constants
    
    private const string STRING_FORMAT = "yyyy-MM-ddTHH:mm:ss.fffZ";
    
    #endregion
    
    #region Properties
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Newtonsoft.Json.JsonProperty("type")]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public override FieldType Type => FieldType.datetime;
    
    [JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    protected override string StringFormat => STRING_FORMAT;
    
    #endregion
    
    #region Methods
    
    public override object Clone()
    {
        return new DateTimeFieldInfo
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
            MinValue = this.MinValue,
            MaxValue = this.MaxValue,
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