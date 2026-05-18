using System.Text.Json.Serialization;

namespace Ertis.Schema.Types.CustomTypes;

public class JsonFieldInfo : FieldInfo<object>
{
	#region Properties
	
	[JsonPropertyName("type")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public override FieldType Type => FieldType.json;
	
	#endregion
	
	#region Methods
	
	public override object Clone()
	{
		return new JsonFieldInfo
		{
			Name = this.Name,
			Description = this.Description,
			DisplayName = this.DisplayName,
			Parent = this.Parent,
			IsRequired = this.IsRequired,
			IsVirtual = this.IsVirtual,
			IsHidden = this.IsHidden,
			IsReadonly = this.IsReadonly,
			DefaultValue = this.DefaultValue
		};
	}
	
	#endregion
}