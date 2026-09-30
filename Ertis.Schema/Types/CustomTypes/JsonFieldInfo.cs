using System.Text.Json.Serialization;

namespace Ertis.Schema.Types.CustomTypes;

public class JsonFieldInfo : FieldInfo<object>
{
	#region Properties
	
	[JsonPropertyName("type")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	[Newtonsoft.Json.JsonProperty("type")]
	[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
	public override FieldType Type => FieldType.json;
	
	#endregion
}