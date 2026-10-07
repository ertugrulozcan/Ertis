using System.Text.Json.Serialization;
using Ertis.Schema.Types.Primitives;

namespace Ertis.Schema.Types.CustomTypes;

public class LongTextFieldInfo : StringFieldInfo
{
	#region Properties
	
	[JsonPropertyName("type")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public override FieldType Type => FieldType.longtext;
	
	#endregion
}