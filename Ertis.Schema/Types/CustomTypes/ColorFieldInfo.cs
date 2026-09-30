using System.Text.Json.Serialization;
using Ertis.Schema.Types.Primitives;

namespace Ertis.Schema.Types.CustomTypes;

public class ColorFieldInfo : StringFieldInfo
{
	#region Properties
	
	[JsonPropertyName("type")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	[Newtonsoft.Json.JsonProperty("type")]
	[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
	public override FieldType Type => FieldType.color;
	
	#endregion
	
	#region Constructors
	
	/// <summary>
	/// Constructor
	/// </summary>
	public ColorFieldInfo()
	{
		this.RegexPattern = "(?:#|0x)(?:[a-f0-9]{3}|[a-f0-9]{6})\\b|(?:rgb|hsl)a?\\([^\\)]*\\)";
	}
	
	#endregion
	
	#region Methods
	
	protected override string GetRegexPatternErrorMessage()
	{
		return "Color code is not valid";
	}
	
	#endregion
}