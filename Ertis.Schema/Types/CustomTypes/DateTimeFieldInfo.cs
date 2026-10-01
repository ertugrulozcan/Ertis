using System.Text.Json.Serialization;

namespace Ertis.Schema.Types.CustomTypes;

public class DateTimeFieldInfo : DateTimeFieldInfoBase
{
	#region Constants
	
	private const string STRING_FORMAT = "yyyy-MM-ddTHH:mm:ss.fffZ";
	
	private static readonly string[] ACCEPTED_FORMATS = ["yyyy-MM-ddTHH:mm:ssK", "yyyy-MM-ddTHH:mm:ss.FFFFFFFK"];
	
	#endregion
	
	#region Properties
	
	[JsonPropertyName("type")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public override FieldType Type => FieldType.datetime;
	
	[JsonIgnore]
	protected override string StringFormat => STRING_FORMAT;
	
	/// <summary>
	/// ISO 8601 date times with 'Z', with an offset or without an offset (UTC), with or without fractional seconds
	/// </summary>
	[JsonIgnore]
	protected override string[] AcceptedFormats => ACCEPTED_FORMATS;
	
	#endregion
}