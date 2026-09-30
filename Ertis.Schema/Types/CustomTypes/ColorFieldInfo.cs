using System.Text.Json.Serialization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Validation;

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
	
	protected internal override bool Validate(object? obj, IValidationContext validationContext)
	{
		try
		{
			return base.Validate(obj, validationContext);
		}
		catch (FieldValidationException ex)
		{
			validationContext.Errors.Add(
				ex.Message == $"String value is not valid by the regular expression rule. ('{this.RegexPattern}')"
					? new FieldValidationException("Color code is not valid", this)
					: ex);
			
			return false;
		}
	}
	
	#endregion
}