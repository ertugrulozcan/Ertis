using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Validation;

namespace Ertis.Schema.Types.CustomTypes;

public partial class HostNameFieldInfo : StringFieldInfo
{
	#region Properties
	
	[JsonPropertyName("type")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public override FieldType Type => FieldType.hostname;
	
	#endregion
	
	#region Methods
	
	protected internal override bool Validate(object? obj, IValidationContext validationContext)
	{
		var isValid = base.Validate(obj, validationContext);
		
		if (obj is string hostName)
		{
			if (!IsValidHostName(hostName))
			{
				isValid = false;
				validationContext.Errors.Add(new FieldValidationException("Hostname is not valid", this));
			}
		}
		
		return isValid;
	}
	
	/// <summary>
	/// A host name (labels of letters, digits and hyphens; unicode names are checked by their ASCII form) or an IPv4 address.
	/// Schemes, paths, ports, single-label names (localhost), IPv6 addresses and trailing dots are not host names here.
	/// </summary>
	private static bool IsValidHostName(string hostName)
	{
		if (string.IsNullOrWhiteSpace(hostName))
		{
			return false;
		}
		
		if (Ipv4Regex().IsMatch(hostName))
		{
			return true;
		}
		
		string asciiHostName;
		try
		{
			asciiHostName = new IdnMapping().GetAscii(hostName);
		}
		catch (ArgumentException)
		{
			return false;
		}
		
		if (asciiHostName.Length > 253)
		{
			return false;
		}
		
		var labels = asciiHostName.Split('.');
		if (labels.Length < 2 || !labels.All(x => LabelRegex().IsMatch(x)))
		{
			return false;
		}
		
		// A numeric top-level label is an invalid IP address, not a host name
		return !labels[^1].All(char.IsDigit);
	}
	
	[GeneratedRegex("^[A-Za-z0-9](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?$")]
	private static partial Regex LabelRegex();
	
	[GeneratedRegex(@"^(?:(?:25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)\.){3}(?:25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)$")]
	private static partial Regex Ipv4Regex();
	
	#endregion
}