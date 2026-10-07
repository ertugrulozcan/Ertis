using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Validation;

namespace Ertis.Schema.Types.CustomTypes;

public partial class ColorFieldInfo : StringFieldInfo
{
	#region Properties
	
	[JsonPropertyName("type")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public override FieldType Type => FieldType.color;
	
	/// <summary>
	/// The color is validated by the built-in rule below; a regexPattern (e.g. stored by the older versions) is ignored
	/// </summary>
	protected override bool UsesRegexPattern => false;
	
	#endregion
	
	#region Methods
	
	protected internal override bool Validate(object? obj, IValidationContext validationContext)
	{
		var isValid = base.Validate(obj, validationContext);
		
		if (obj is string color && !string.IsNullOrEmpty(color) && !ColorRegex().IsMatch(color))
		{
			isValid = false;
			validationContext.Errors.Add(new FieldValidationException("Color code is not valid", this));
		}
		
		return isValid;
	}
	
	/// <summary>
	/// #RGB, #RGBA, #RRGGBB, #RRGGBBAA, 0xRGB, 0xRRGGBB and rgb/rgba/hsl/hsla with 3 or 4 numeric (or percentage) arguments
	/// </summary>
	[GeneratedRegex(@"^(?:#(?:[0-9a-f]{3,4}|[0-9a-f]{6}|[0-9a-f]{8})|0x(?:[0-9a-f]{3}|[0-9a-f]{6})|(?:rgba?|hsla?)\(\s*-?\d+(?:\.\d+)?%?\s*(?:,\s*-?\d+(?:\.\d+)?%?\s*){2,3}\))$", RegexOptions.IgnoreCase)]
	private static partial Regex ColorRegex();
	
	#endregion
}