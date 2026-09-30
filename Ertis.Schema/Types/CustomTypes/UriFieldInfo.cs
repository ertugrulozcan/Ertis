using System.Text.Json.Serialization;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Validation;

namespace Ertis.Schema.Types.CustomTypes;

public class UriFieldInfo : StringFieldInfo
{
	#region Properties
	
	[Newtonsoft.Json.JsonProperty("type")]
	[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
	[JsonPropertyName("type")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public override FieldType Type => FieldType.uri;
	
	#endregion
	
	#region Methods
	
	protected internal override bool Validate(object? obj, IValidationContext validationContext)
	{
		var isValid = base.Validate(obj, validationContext);
		
		if (obj is string uri)
		{
			if (!IsValidUri(uri))
			{
				isValid = false;
				validationContext.Errors.Add(new FieldValidationException("Uri is not valid", this));
			}
		}
		
		return isValid;
	}
	
	private static bool IsValidUri(string uri)
	{
		if (string.IsNullOrWhiteSpace(uri))
		{
			return false;
		}
		
		return Uri.IsWellFormedUriString(uri, UriKind.Absolute) && Uri.TryCreate(uri, UriKind.Absolute, out _);
	}
	
	#endregion
}