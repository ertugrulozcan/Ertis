using System.Text.Json.Serialization;

namespace Ertis.Schema.Types.CustomTypes;

public class JsonFieldInfo : FieldInfo<object>
{
	#region Properties
	
	[JsonPropertyName("type")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public override FieldType Type => FieldType.json;
	
	#endregion
}