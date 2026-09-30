using System.Text.Json.Serialization;

namespace Ertis.Schema.Types.CustomTypes;

public class DateFieldInfo : DateTimeFieldInfoBase
{
	#region Constants
	
	private const string STRING_FORMAT = "yyyy-MM-dd";
	
	#endregion
	
	#region Properties
	
	[JsonPropertyName("type")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	[Newtonsoft.Json.JsonProperty("type")]
	[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
	public override FieldType Type => FieldType.date;
	
	[JsonIgnore]
	[Newtonsoft.Json.JsonIgnore]
	protected override string StringFormat => STRING_FORMAT;
	
	#endregion
}