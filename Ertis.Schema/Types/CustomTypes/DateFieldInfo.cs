using System.Text.Json.Serialization;

namespace Ertis.Schema.Types.CustomTypes;

public class DateFieldInfo : DateTimeFieldInfoBase<DateFieldInfo>
{
	#region Constants
	
	private const string STRING_FORMAT = "yyyy-MM-dd";
	
	#endregion
	
	#region Properties
	
	[JsonPropertyName("type")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public override FieldType Type => FieldType.date;
	
	[JsonIgnore]
	protected override string StringFormat => STRING_FORMAT;
	
	#endregion
}