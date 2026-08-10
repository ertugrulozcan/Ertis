using System.Text.Json.Serialization;
using Ertis.Schema.Dynamics.Legacy;
using Ertis.Schema.Types.Primitives;

namespace Ertis.Schema.Types.CustomTypes;

public sealed class CodeFieldInfo : ObjectFieldInfoBase
{
	#region Properties
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Newtonsoft.Json.JsonProperty("type")]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public override FieldType Type => FieldType.code;
    
    [JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public override IReadOnlyCollection<IFieldInfo> Properties { get; init; }
    
    #endregion
    
    #region Constructors
    
    /// <summary>
    /// Constructor
    /// </summary>
    public CodeFieldInfo()
    {
        this.Properties = new[]
        {
            new StringFieldInfo
            {
                Name = "code",
                DisplayName = "Code",
                Description = "Code",
                IsRequired = true
            },
            new StringFieldInfo
            {
                Name = "language",
                DisplayName = "Language",
                Description = "Programming or Script Language",
                IsRequired = true
            }
        };
    }
    
    #endregion
    
    #region Methods
    
    public override object? GetDefaultValue()
    {
        var defaultValue = base.GetDefaultValue();
        if (defaultValue is Newtonsoft.Json.Linq.JObject jObject)
        {
            return DynamicObject.Load(jObject).ToDynamic();
        }
        else
        {
            return defaultValue;
        }
    }
    
    public override object Clone()
    {
        return new CodeFieldInfo
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
            Properties = this.Properties
        };
    }
    
    #endregion
}