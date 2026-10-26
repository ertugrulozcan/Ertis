using System.Text.Json.Serialization;

namespace Ertis.Schema.Types.CustomTypes;

public class DateFieldInfo : DateTimeFieldInfoBase
{
	#region Constants
	
	private const string STRING_FORMAT = "yyyy-MM-dd";
	
	private static readonly string[] ACCEPTED_FORMATS = [STRING_FORMAT, "yyyy-MM-ddTHH:mm:ssK", "yyyy-MM-ddTHH:mm:ss.FFFFFFFK"];
	
	#endregion
	
	#region Properties
	
	[JsonPropertyName("type")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public override FieldType Type => FieldType.date;
	
	[JsonIgnore]
	protected override string StringFormat => STRING_FORMAT;
	
	[JsonIgnore]
	protected override string[] AcceptedFormats => ACCEPTED_FORMATS;
	
	#endregion
}