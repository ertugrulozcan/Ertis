using System.Collections.Generic;
using Ertis.Schema.Types.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ertis.Schema.Types.CustomTypes;

public class NestedTypeFieldInfo : ObjectFieldInfoBase
{
	#region Properties
    
    [JsonProperty("type")]
    [JsonConverter(typeof(StringEnumConverter))]
    public override FieldType Type => FieldType.nestedType;
    
    [JsonIgnore]
    public override IReadOnlyCollection<IFieldInfo> Properties { get; init; }
    
    [JsonProperty("nestedTypeId")]
    public string NestedTypeId { get; set; }

    #endregion
    
    #region Methods

    public override object Clone()
    {
        return new NestedTypeFieldInfo
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
            Properties = this.Properties,
        };
    }

    #endregion
}